using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
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
        UnitCardSO[] allCardSOs = Resources.LoadAll<UnitCardSO>("ScriptableObjects/Cards/Greek/UnitCards");
        List<Card> allCards = new List<Card>();
        foreach (UnitCardSO cardSO in allCardSOs)
        {
            allCards.Add(new UnitCard(cardSO));
        }

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in allCards)
        {
            var cardInstance = Instantiate(CardPrefab, ScrollContent);
            var cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCardData(card);
        }

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        foreach (var kvp in playerData.CardCollection)
        {
            string cardId = kvp.Key;
            int cardQuantity = kvp.Value;
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
