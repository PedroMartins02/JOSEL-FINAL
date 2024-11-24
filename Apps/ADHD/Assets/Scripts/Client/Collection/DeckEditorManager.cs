using GameModel;
using ModestTree;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditorManager : MonoBehaviour
{
    public static DeckEditorManager Instance { get; private set; } //not ideal, lol
    
    [SerializeField] private Transform ScrollContent;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private GameObject MythPrefab;
    [SerializeField] private GameObject QuantityPrefab;
    [SerializeField] private Transform DeckCardTemplate;
    [SerializeField] private Transform MythListPrefab;
    [SerializeField] private Transform EditingAreaPrefab;
    [SerializeField] private HighlightedDeckIdSO HighlightedDeckData;
    [SerializeField] private TextMeshProUGUI DeckName;

    private Factions faction = 0;
    private List<CardSO> ListOfSelectedCards = new List<CardSO>();
    private DeckData playerCurrentDeck = null;
    private CardSO selectedMyth = null;
    
    

    private Dictionary<Factions, bool> factionsFilter = new Dictionary<Factions, bool>();
    private Dictionary<int, bool> blessingsFilter = new Dictionary<int, bool>();
    private Dictionary<Type, bool> typeFilter = new Dictionary<Type, bool>();
    
    private Dictionary<int, Type> cardTypeMapping = new Dictionary<int, Type>();

    private void Awake()
    {
        Instance = this;
        InitializeCardTypeMapping();
        InitializeFilters();
        DeckCardTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Setting the faction
        this.faction = (Factions)PlayerPrefs.GetInt("ChosenFaction", 0); // Default to 0 (Greek)
        
        Debug.Log("Chosen Faction: " + this.faction);

        UpdateCardsList();
        if(playerCurrentDeck != null) OnDeckLoad(playerCurrentDeck.CardList);
    }

    private void UpdateCardsList()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        
        if (playerData.DeckCollection.Count >= HighlightedDeckData.DeckId) playerCurrentDeck = playerData.DeckCollection[HighlightedDeckData.DeckId];

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in MythListPrefab)
        {
            Destroy(child.gameObject);
        }

        List<string> filteredCardCollection = new List<string>(playerData.CardCollection);
        foreach (var cardIdToRemove in playerCurrentDeck.CardList)
        {
            filteredCardCollection.Remove(cardIdToRemove);
        }
        
        Dictionary<string, int> cardCount = filteredCardCollection
            .GroupBy(item => item)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in cardCount)
        {
            CardSO card = CardDatabase.Singleton.GetCardSoOfId(kvp.Key);
            if (card.GetType() == typeof(MythCardSO))
            {
                var mythCardInstance = Instantiate(MythPrefab, MythListPrefab);
                var mythCardUI = mythCardInstance.GetComponent<MythUI>();
                selectedMyth = card;
                mythCardUI.SetMythData((MythCardSO) card);
            }

            if (!FilterCard(card))
            {
                continue;
            }
            var cardInstance = Instantiate(CardPrefab, ScrollContent);
            var cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCardData(card);

            var quantityInstance = Instantiate(QuantityPrefab, cardInstance.transform);
            var quantityUI = quantityInstance.GetComponent<QuantityUI>();
            quantityUI.SetQuantity(kvp.Value);

            Button cardButton = cardInstance.GetComponent<Button>();

            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => AddToEditingArea(card));
            }
        }
        
        
    }
    
    private void UpdateCardsList(List<string> tempCardList)
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        
        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in MythListPrefab)
        {
            Destroy(child.gameObject);
        }

        List<string> filteredCardCollection = new List<string>(playerData.CardCollection);
        
        foreach (var cardIdToRemove in tempCardList)
        {
            filteredCardCollection.Remove(cardIdToRemove);
        }
        
        Dictionary<string, int> cardCount = filteredCardCollection
            .GroupBy(item => item)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in cardCount)
        {
            CardSO card = CardDatabase.Singleton.GetCardSoOfId(kvp.Key);
            if (card.GetType() == typeof(MythCardSO))
            {
                var mythCardInstance = Instantiate(MythPrefab, MythListPrefab);
                var mythCardUI = mythCardInstance.GetComponent<CardUI>();
                selectedMyth = card;
                mythCardUI.SetCardData(card);
                Button mythCardButton = mythCardInstance.GetComponent<Button>();
                if (mythCardButton != null)
                {
                    mythCardButton.onClick.RemoveAllListeners();
                    mythCardButton.onClick.AddListener(() => SelectMyth(card));
                }
            }

            if (!FilterCard(card))
            {
                continue;
            }
            var cardInstance = Instantiate(CardPrefab, ScrollContent);
            var cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCardData(card);

            var quantityInstance = Instantiate(QuantityPrefab, cardInstance.transform);
            var quantityUI = quantityInstance.GetComponent<QuantityUI>();
            quantityUI.SetQuantity(kvp.Value);

            Button cardButton = cardInstance.GetComponent<Button>();

            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => AddToEditingArea(card));
            }
        }
        
        
    }
    

    private void InitializeCardTypeMapping()
    {
        cardTypeMapping = new Dictionary<int, Type>
        {
            { 0, typeof(UnitCardSO) },
            { 1, typeof(BattleTacticCardSO) },
            { 2, typeof(LegendCardSO) }
        };
    }

    private void InitializeFilters()
    {
        factionsFilter = new Dictionary<Factions, bool>();
        foreach (Factions faction in Enum.GetValues(typeof(Factions)))
        {
            factionsFilter.Add(faction, true);
        }

        blessingsFilter = new Dictionary<int, bool>();
        for (int i = 0; i < 11; i++)
        {
            blessingsFilter.Add(i, true);
        }

        typeFilter = new Dictionary<Type, bool>
        {
            { typeof(UnitCardSO), true },
            { typeof(BattleTacticCardSO), true },
            { typeof(LegendCardSO), true},
        };
    }

    private bool FilterCard(CardSO card)
    {
        if (factionsFilter.TryGetValue(card.Faction, out bool factionAllowed) && !factionAllowed)
            return false;

        if (blessingsFilter.TryGetValue(card.Blessings, out bool blessingsAllowed) && !blessingsAllowed)
            return false;

        if (typeFilter.TryGetValue(card.GetType(), out bool typeAllowed) && !typeAllowed)
            return false;

        return true;
    }

    public void ChangeFactionFilter(int faction)
    {
        factionsFilter[(Factions)faction] = !factionsFilter[(Factions)faction];
        UpdateCardsList(ListOfSelectedCards.Select(e => e.Id).ToList());
    }

    public void ChangeBlessingsFilter(int blessings)
    {
        blessingsFilter[blessings] = !blessingsFilter[blessings];
        UpdateCardsList(ListOfSelectedCards.Select(e => e.Id).ToList());
    }

    public void ChangeTypeFilter(int type)
    {
        cardTypeMapping.TryGetValue(type, out Type cardType);
        if (cardType == null)
        {
            return;
        }
        typeFilter[cardType] = !typeFilter[cardType];
        UpdateCardsList(ListOfSelectedCards.Select(e => e.Id).ToList());
    }

    private void OnDeckLoad(List<string> cardsToLoad)
    {
        DeckName.text = playerCurrentDeck.Name;
        
        foreach (var id in cardsToLoad)
        {
            CardSO card = CardDatabase.Singleton.GetCardSoOfId(id);
            AddToEditingArea(card);
        }
    }
    

    private void AddToEditingArea(CardSO cardSO)
    {
        Transform cardInstance = Instantiate(DeckCardTemplate, EditingAreaPrefab);
        cardInstance.gameObject.SetActive(true);
        ListOfSelectedCards.Add(cardSO);
        UpdateCardsList(ListOfSelectedCards.Select(e => e.Id.ToString()).ToList());
        DeckCardUI deckCardUI = cardInstance.GetComponent<DeckCardUI>();
        deckCardUI.SetCardData(cardSO);
        
    }

    public void RemoveFromEditingArea(CardSO cardToRemove)
    {
        ListOfSelectedCards.Remove(cardToRemove);
        List<string> stringIdList = ListOfSelectedCards.Select(e => e.Id.ToString()).ToList();
        UpdateCardsList(stringIdList);
    }

    public void SelectMyth(CardSO myth)
    {
        selectedMyth = myth;
    }

    public void SaveDeck()
    {
        AccountManager.Singleton.AddDeckToPlayer(new DeckSO(DeckName.text,selectedMyth,ListOfSelectedCards,0));
    }

    public void BackButton()
    {
        SceneLoader.Load(SceneLoader.Scene.NavigationScene);
    }
    
}
