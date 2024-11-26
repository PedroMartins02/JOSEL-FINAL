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
    [SerializeField] private TMP_InputField deckNameInput;

    private Factions faction;
    private DeckData playerCurrentDeck;
    private string deckName;
    private int slotIndex;
    private int deckCardBackID = 0; //Variable to store chosen deck back customization.

    private List<CardSO> ListOfSelectedCards = new List<CardSO>();
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
        // Verify if its creating a new deck (value = 0) or not (value = 1)
        List<DeckData> playerDeckData = AccountManager.Singleton.GetPlayerData().DeckCollection;
        
        if (PlayerPrefs.GetInt("isNewDeck", 0) == 1)
        {
            // Get the faction and the deck
            foreach (DeckData deck in playerDeckData)
            {
                if (deck.Name.Equals(PlayerPrefs.GetString("ChosenDeckName", null)))
                {
                    CardSO firstCard = CardDatabase.Singleton.GetCardSoOfId(deck.CardList[1]);
                    this.faction = firstCard.Faction;
                    deckCardBackID = deck.CardBackId;
                    this.playerCurrentDeck = deck;
                    OnDeckLoad(deck);
                }
            }
        } else
        {
            this.faction = (Factions)PlayerPrefs.GetInt("ChosenFaction", 0); // Default to 0 (Greek)

            DeckData newDeck = new DeckData(); ;
            this.playerCurrentDeck = newDeck;
            OnDeckLoad(newDeck);
        }

        Debug.Log("FACTION DETECTED: " + faction);

        // Set the slot index that was click in the menu
        this.slotIndex = PlayerPrefs.GetInt("SlotIndex", 0);
        Debug.Log("SLOT DETECTED: " + slotIndex);

        // Change Faction for the deck
        ChangeFactionFilter((int)this.faction);
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
        List<string> filteredCardCollection = new List<string>(playerData.CardCollection);

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
        RectTransform mythContRect = mythContainer.GetComponent<RectTransform>();
        RectTransform mythTemplateRect = mythTemplate.GetComponent<RectTransform>();
        float mythTotalHeight = mythTemplateRect.rect.height * mythCardsCount + 60 * (mythCardsCount - 1);
        mythContRect.sizeDelta = new Vector2(mythContRect.sizeDelta.x, mythTotalHeight);
        // Set the size for the card collection scroll
        RectTransform colecContRect = cardCollectionContainer.GetComponent<RectTransform>();
        RectTransform colecTempRect = cardCollectionTemplate.GetComponent<RectTransform>();
        int aproxRows = DivideRoundingUp(colecCardsCount, 3);
        float colecTotalHeight = colecTempRect.rect.height * aproxRows + 40 * (aproxRows - 1);
        colecContRect.sizeDelta = new Vector2(colecContRect.sizeDelta.x, colecTotalHeight);
        // Set the size for the selected cards scroll
        RectTransform editContRect = selectedCardsContainer.GetComponent<RectTransform>();
        RectTransform editTempRect = selectedCardsTemplate.GetComponent<RectTransform>();
        float totalHeight = 115 * ListOfSelectedCards.Count + 30 + 10 * (ListOfSelectedCards.Count - 1);
        editContRect.sizeDelta = new Vector2(editContRect.sizeDelta.x, totalHeight);
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
            factionsFilter.Add(faction, false);
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

    private void OnDeckLoad(DeckData deck)
    {
        deckNameInput.text = deck.Name;
        
        foreach (var id in deck.CardList)
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
        this.deckName = deckNameInput.text.IsEmpty() ? "New Deck" : deckNameInput.text;
        AccountManager.Singleton.AddDeckToPlayer(this.slotIndex, this.deckCardBackID, new DeckSO(deckName,selectedMyth,ListOfSelectedCards,this.faction));
        BackButton();
    }

    public void BackButton()
    {
        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }
    
}
