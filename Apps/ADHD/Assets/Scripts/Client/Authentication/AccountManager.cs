using GameModel;
using ModestTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    private static AccountManager _singleton;
    public static AccountManager Singleton
    {
        get => _singleton;
        private set
        {
            if (null == _singleton)
                _singleton = value;
            else if (value != _singleton)
            {
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private PlayerData playerData;
    public Elements WeatherElement { get; set; }
    public Elements TimeElement { get; set; }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerData playerData, bool isToSave = false)
    {
        this.playerData = playerData;
        if (isToSave)
        {
            SaveData();
        }
    }

    public async Task<bool> LoadData()
    {
        var playerDataDictionary = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {"playerData"});

        if (playerDataDictionary.TryGetValue("playerData", out var firstKey))
        {
            playerData = PlayerData.FromDictionary(firstKey.Value.GetAs<Dictionary<string, object>>());
            Debug.Log($"Loaded data {string.Join(',', playerDataDictionary)}");
            Debug.Log($"PlayerData instance {playerData.ToJson()}");
            VerifyData();
            return true;
        }

        if (playerData != null)
        {
            VerifyData();
        }

        return false;
    }

    private async void SaveData()
    {
        Debug.Log($"PlayerData instance {playerData.ToJson()}");
        var playerDataDictionary = new Dictionary<string, object>{
          {"playerData", playerData.ToDictionary()}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerDataDictionary);
        Debug.Log($"Saved data {string.Join(',', playerDataDictionary)}");
    }


    private void VerifyData()
    {
        bool dataChanged = false;
        if (playerData.CardCollection.IsEmpty())
        {
            AwardStarterDeckCards();
            dataChanged = true;
        }
        if (playerData.DeckCollection.IsEmpty())
        {
            AwardStarterDeck();
            dataChanged = true;
        }
        if (playerData.CardBackCollection.IsEmpty())
        {
            playerData.CardBackCollection.Add(0);
            dataChanged = true;
        }
        LoadAllMyths(playerData.CardCollection);
        SetPlayerData(playerData, dataChanged);
    }

    private void LoadAllMyths(List<string> currentCollection)
    {
        List<CardSO> myths = CardDatabase.Singleton.GetAllMyths();

        foreach (CardSO myth in myths)
        {
            if (!currentCollection.Contains(myth.Id))
            {
                currentCollection.Add(myth.Id);
            }
        }
    }

    private DeckSO LoadStarterDeckCards()
    {
        DeckSO[] deckSOs = Resources.LoadAll<DeckSO>("ScriptableObjects/Decks");
        return deckSOs.Where(deck => deck.Name.Equals("Starter Deck")).First();
    }

    private void AwardStarterDeckCards()
    {
        DeckSO starterDeck = LoadStarterDeckCards();
        foreach (CardSO card in starterDeck.Cards)
        {
            playerData.CardCollection.Add(card.Id);
        }
        playerData.CardCollection.Add(starterDeck.Myth.Id);
    }

    private void AwardStarterDeck()
    {
        DeckSO starterDeck = LoadStarterDeckCards();
        if (playerData.DeckCollection.IsEmpty())
        {
            playerData.DeckCollection.Add(new DeckData(starterDeck));
        }
        playerData.SelectedDeckId = 0;
    }

    public void AddDeckToPlayer(int DeckSlotIndex, int cardBackID, DeckSO newDeck)
    {
        Debug.Log("colection size = " + playerData.DeckCollection.Count);
        Debug.Log("slot index = " + DeckSlotIndex);
        if (playerData.DeckCollection.Count > DeckSlotIndex)
        {
            Debug.Log("save edit reached");
            EditExistingDeck(DeckSlotIndex, newDeck, cardBackID);
            return;
        }

        DeckData newDeckData = new DeckData(newDeck, cardBackID);
        playerData.DeckCollection.Add(newDeckData);
        SaveData();
    }

    public void EditExistingDeck(int id, DeckSO deckToEdit, int cardBackId)
    {
        playerData.DeckCollection[id] = new DeckData(deckToEdit, cardBackId);
    }

    public void DeleteDeck(DeckSO deckToDelete)
    {
        playerData.DeckCollection.Remove(new DeckData(deckToDelete));
    }
}
