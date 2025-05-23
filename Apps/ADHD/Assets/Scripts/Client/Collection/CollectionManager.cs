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

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private Transform ScrollContent;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private GameObject QuantityPrefab;

    [SerializeField] private GameObject PopUp;
    [SerializeField] private Transform PopUpSlot;

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
        DisablePopUp();
        UpdateCardsList();
    }

    private void UpdateCardsList()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }

        List<CardSO> filteredCardCollection = new List<CardSO>();
        foreach (string cardId in playerData.CardCollection)
        {
            filteredCardCollection.Add(CardDatabase.Singleton.GetCardSoOfId(cardId));
        }

        filteredCardCollection = filteredCardCollection
            .OrderBy(card =>
            {
                if (card is LegendCardSO) return 1;
                if (card is UnitCardSO) return 2;
                if (card is BattleTacticCardSO) return 3;
                return int.MaxValue;
            })
            .ThenBy(card => card.Blessings)
            .ThenBy(card => card.Name)
            .ToList();

        Dictionary<CardSO, int> cardCount = filteredCardCollection
            .GroupBy(item => item)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in cardCount)
        {
            CardSO card = kvp.Key;
            if (card.GetType() == typeof(MythCardSO))
            {
                continue;
            }

            if (!FilterCard(card))
            {
                continue;
            }
            var cardInstance = Instantiate(CardPrefab, ScrollContent);
            var cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCardData(card);

            var quantityInstace = Instantiate(QuantityPrefab, cardInstance.transform);
            var quantityUI = quantityInstace.GetComponent<QuantityUI>();
            quantityUI.SetQuantity(kvp.Value);

            Button cardButton = cardInstance.GetComponent<Button>();

            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => ShowCard(card));
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
        UpdateCardsList();
    }

    public void ChangeBlessingsFilter(int blessings)
    {
        blessingsFilter[blessings] = !blessingsFilter[blessings];
        UpdateCardsList();
    }

    public void ChangeTypeFilter(int type)
    {
        cardTypeMapping.TryGetValue(type, out Type cardType);
        if (cardType == null)
        {
            return;
        }
        typeFilter[cardType] = !typeFilter[cardType];
        UpdateCardsList();
    }

    public void DisablePopUp()
    {
        PopUp.SetActive(false);
        ClearPopUpArea();
    }

    private void ShowCard(CardSO cardSO)
    {
        ClearPopUpArea();
        var cardInstance = Instantiate(CardPrefab, PopUpSlot);
        var cardUI = cardInstance.GetComponent<CardUI>();
        cardUI.SetCardData(cardSO);
        PopUp.SetActive(true);
    }

    private void ClearPopUpArea()
    {
        foreach (Transform child in PopUpSlot)
        {
            Destroy(child.gameObject);
        }
    }
}
