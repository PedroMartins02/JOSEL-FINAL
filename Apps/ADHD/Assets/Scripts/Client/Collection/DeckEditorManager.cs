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

    [Header("Card Collection Section")]
    [SerializeField] private Transform cardCollectionContainer;
    [SerializeField] private Transform cardCollectionTemplate;

    [Header("Myth Section")]
    [SerializeField] private Transform mythContainer;
    [SerializeField] private Transform mythTemplate;

    [Header("Selected Card Section")]
    [SerializeField] private Transform selectedCardsContainer;
    [SerializeField] private Transform selectedCardsTemplate;

    [Header("Other")]
    [SerializeField] private GameObject QuantityPrefab;
    [SerializeField] private HighlightedDeckIdSO HighlightedDeckData;
    [SerializeField] private TextMeshProUGUI DeckName;

    private Factions faction;
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

        cardCollectionTemplate.gameObject.SetActive(false);
        mythTemplate.gameObject.SetActive(false);
        selectedCardsTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Setting the faction
        this.faction = (Factions)PlayerPrefs.GetInt("ChosenFaction", 0); // Default to 0 (Greek)
        
        // Change Faction for the deck
        ChangeFactionFilter((int)this.faction);

        if(playerCurrentDeck != null) OnDeckLoad(playerCurrentDeck.CardList);
    }
    
    private void UpdateCardsList(List<string> tempCardList)
    {
        // Get the player data for the cards
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        
        // Clear the content for the cards and myths from the collection
        foreach (Transform child in cardCollectionContainer)
        {
            if (child == cardCollectionTemplate) continue;
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in mythContainer)
        {
            if (child == mythTemplate) continue;
            Destroy(child.gameObject);
        }

        // We need to remove certain cards from the list of the player's collection if they are already selected
        List<string> filteredCardCollection = playerData.CardCollection;

        foreach (var cardIdToRemove in tempCardList)
        {
            filteredCardCollection.Remove(cardIdToRemove);
        }

        Dictionary<string, int> cardCount = filteredCardCollection
            .GroupBy(item => item)
            .ToDictionary(g => g.Key, g => g.Count());

        // Filter and instanciate the cards into the container
        int mythCardsCount = 0;
        int colecCardsCount = 0;

        foreach (var kvp in cardCount)
        {
            CardSO card = CardDatabase.Singleton.GetCardSoOfId(kvp.Key);

            // Filter the card: if its not from the chosen faction, skip it
            if (!FilterCard(card))
            {
                continue;
            }

            // If its a myth card from the faction, add it to the myth section and set it's data
            if (card.GetType() == typeof(MythCardSO))
            {
                Transform mythCardInstance = Instantiate(mythTemplate, mythContainer);
                mythCardInstance.gameObject.SetActive(true);
                MythUI mythCardUI = mythCardInstance.GetComponent<MythUI>();

                selectedMyth = card;

                mythCardUI.SetMythData((MythCardSO)card);
                Transform selected = mythCardUI.transform.Find("Selected");
                selected.gameObject.SetActive(false);
                Button mythCardButton = mythCardInstance.GetComponent<Button>();
                if (mythCardButton != null)
                {
                    mythCardButton.onClick.RemoveAllListeners();
                    mythCardButton.onClick.AddListener(() => SelectMyth(mythCardInstance.gameObject, card));
                }
                mythCardsCount++;
                continue;
            }

            // If reaches here it's because it is a card for the collection
            var cardInstance = Instantiate(cardCollectionTemplate, cardCollectionContainer);
            cardInstance.gameObject.SetActive(true);
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
            colecCardsCount++;
        }
        Debug.Log("myths: " + mythCardsCount);
        Debug.Log("cards: " + colecCardsCount);
        
        // Lastly, we need to resize the containers for the scroll to work (rafa bad kittie)
        // Set the size for the myth scroll (goofy ahh math version)
        RectTransform mythRect = mythContainer.GetComponent<RectTransform>();
        float mythTotalHeight = mythRect.rect.height * mythCardsCount + 60 * (mythCardsCount - 1);
        mythRect.sizeDelta = new Vector2(mythRect.sizeDelta.x, mythTotalHeight);
        // Set the size for the card collection scroll
        RectTransform colecRect = cardCollectionContainer.GetComponent<RectTransform>();
        int aproxRows = DivideRoundingUp(colecCardsCount, 3);
        float colecTotalHeight = mythRect.rect.height * aproxRows + 40 * (aproxRows - 1);
        mythRect.sizeDelta = new Vector2(mythRect.sizeDelta.x, mythTotalHeight);
    }

    private int DivideRoundingUp(int x, int y)
    {
        int remainder;
        int quotient = Math.DivRem(x, y, out remainder);
        return remainder == 0 ? quotient : quotient + 1;
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

        return true;
    }

    public void ChangeFactionFilter(int faction)
    {
        factionsFilter[(Factions)faction] = !factionsFilter[(Factions)faction];
        UpdateCardsList(new List<string>());
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
        Transform cardInstance = Instantiate(selectedCardsTemplate, selectedCardsContainer);
        cardInstance.gameObject.SetActive(true);
        ListOfSelectedCards.Add(cardSO);
        UpdateCardsList(ListOfSelectedCards.Select(e => e.Id.ToString()).ToList());
        DeckCardUI deckCardUI = cardInstance.GetComponent<DeckCardUI>();
        deckCardUI.SetCardData(cardSO);

        // Set the size for the selected cards scroll
        RectTransform editRect = selectedCardsContainer.GetComponent<RectTransform>();
        float totalHeight = editRect.rect.height * ListOfSelectedCards.Count + 60 * (ListOfSelectedCards.Count - 1);
        editRect.sizeDelta = new Vector2(editRect.sizeDelta.x, totalHeight);
    }

    public void RemoveFromEditingArea(CardSO cardToRemove)
    {
        ListOfSelectedCards.Remove(cardToRemove);
        List<string> stringIdList = ListOfSelectedCards.Select(e => e.Id.ToString()).ToList();
        UpdateCardsList(stringIdList);
    }

    private void SelectMyth(GameObject mythCardUI, CardSO myth)
    {
        selectedMyth = myth;
        Transform selected = mythCardUI.transform.Find("Selected");
        selected.gameObject.SetActive(true);

    }

    public void SaveDeck()
    {
        AccountManager.Singleton.AddDeckToPlayer(HighlightedDeckData.DeckId,new DeckSO(DeckName.text,selectedMyth,ListOfSelectedCards,0));
        BackButton();
    }

    public void BackButton()
    {
        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }
    
}
