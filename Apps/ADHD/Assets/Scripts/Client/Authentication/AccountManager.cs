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
        if (playerData.CardCollection.IsEmpty())
        {
            AwardStarterDeck();
        }
    }

    private void AwardStarterDeck()
    {
        DeckSO[] deckSOs = Resources.LoadAll<DeckSO>("ScriptableObjects/Decks");
        DeckSO starterDeck = deckSOs.Where(deck => deck.Name.Equals("Starter Deck")).First();
        foreach (CardSO card in starterDeck.Cards)
        {
            playerData.CardCollection.Add(card.Id);
        }
        playerData.CardCollection.Add(starterDeck.Myth.Id);

        if (playerData.DeckCollection.IsEmpty())
        {
            playerData.DeckCollection.Add(new DeckData(starterDeck));
        }

        SetPlayerData(playerData, true);
    }
}
