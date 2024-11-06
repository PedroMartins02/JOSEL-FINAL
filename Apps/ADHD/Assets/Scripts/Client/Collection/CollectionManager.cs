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

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private Transform ScrollContent;
    [SerializeField] private GameObject CardPrefab;

    private List<Card> cards = new List<Card>();
    private Dictionary<Factions, bool> factionsFilter = new Dictionary<Factions, bool>();
    private Dictionary<int, bool> blessingsFilter = new Dictionary<int, bool>();
    private Dictionary<Type, bool> typeFilter = new Dictionary<Type, bool>();
    
    private Dictionary<int, Type> cardTypeMapping = new Dictionary<int, Type>();

    private void Awake()
    {
        InitializeCardTypeMapping();
        InitializeFilters();
    }

    private void Start()
    {
        //JUST A TEST
        LoadCardCollection();
    }

    private void LoadCardCollection()
    {
        //JUST A TEST
        UnitCardSO[] allUnitCardSOs = Resources.LoadAll<UnitCardSO>("ScriptableObjects/Cards/Greek/UnitCards");
        List<Card> allUnitCards = new List<Card>();
        foreach (UnitCardSO cardSO in allUnitCardSOs)
        {
            allUnitCards.Add(new UnitCard(cardSO));
        }
        //JUST A TEST

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();

        if (playerData.CardCollection.IsEmpty())
        {
            DeckSO[] deckSOs = Resources.LoadAll<DeckSO>("ScriptableObjects/Decks");
            DeckSO starterDeck = deckSOs.Where(deck => deck.Name.Equals("Starter Deck")).First();
            foreach(CardSO card in starterDeck.Cards)
            {
                playerData.CardCollection.Add(card.Id);
            }

            if (playerData.DeckCollection.IsEmpty())
            {
                playerData.DeckCollection.Add(new DeckData(starterDeck));
            }

            AccountManager.Singleton.SetPlayerData(playerData, true);
        }

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> cardCount = playerData.CardCollection
            .GroupBy(item => item)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in cardCount)
        {
            try
            {
                Card card = new UnitCard(allUnitCardSOs.Where(cardSO => cardSO.Id.Equals(kvp.Key)).First());
                var cardInstance = Instantiate(CardPrefab, ScrollContent);
                var cardUI = cardInstance.GetComponent<CardUI>();
                cardUI.SetCardData(card);
            } catch
            {

            }
        }
    }

    private void InitializeCardTypeMapping()
    {
        cardTypeMapping = new Dictionary<int, Type>
        {
            { 0, typeof(UnitCard) },
            { 1, typeof(BattleTacticCard) },
            //{ 2, typeof(LegendCard) }
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
            { typeof(UnitCard), true },
            { typeof(BattleTacticCard), true },
            //{ typeof(LegendCard), true},
        };
        
    }

    private bool FilterCard(Card card)
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
    }

    public void ChangeBlessingsFilter(int blessings)
    {
        blessingsFilter[blessings] = !blessingsFilter[blessings];
    }

    public void ChangeTypeFilter(int type)
    {
        cardTypeMapping.TryGetValue(type, out Type cardType);
        if (cardType == null)
        {
            return;
        }
        typeFilter[cardType] = !typeFilter[cardType];
    }
}
