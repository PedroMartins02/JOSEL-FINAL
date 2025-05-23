using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Data;

public class CardDatabase : MonoBehaviour
{
    private static CardDatabase _singleton;
    public static CardDatabase Singleton
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

    public Dictionary<string, CardSO> CardDictionary { get; private set; } = new();

    private void Start()
    {
        LoadCards();
    }

    private void LoadCards()
    {
        CardDictionary = Resources.Load<CardDictionarySO>("ScriptableObjects/Dictionary/CardDictionary").GetDictionary();
        Debug.Log($"Loaded {CardDictionary.Count} cards");
    }

    private delegate Card CardConstructor(CardSO cardSO);
    private static readonly Dictionary<Type, CardConstructor> cardConstructors = new Dictionary<Type, CardConstructor>
    {
        { typeof(LegendCardSO), cardSO => new LegendCard((LegendCardData)cardSO.CardData) },
        { typeof(UnitCardSO), cardSO => new UnitCard((UnitCardData)cardSO.CardData) },
        { typeof(BattleTacticCardSO), cardSO => new BattleTacticCard((BattleTacticCardData)cardSO.CardData) },
        { typeof(MythCardSO), cardSO => new MythCard(cardSO.CardData) }
    };

    public Card GetCardOfId(string cardId)
    {
        CardSO cardSO = CardDictionary.ContainsKey(cardId) ? CardDictionary[cardId] : null;
        if (cardSO == null)
            return null;

        if (cardConstructors.TryGetValue(cardSO.GetType(), out CardConstructor constructor))
            return constructor(cardSO);

        return null;
    }

    public CardSO GetCardSoOfId(string cardId)
    {
        return CardDictionary.ContainsKey(cardId) ? CardDictionary[cardId] : null;
    }

    public CardSO GetRandomCard()
    {
        System.Random random = new System.Random();
        var eligibleCards = CardDictionary.Values
        .Where(card => card.GetType() != typeof(MythCardSO))
        .ToList();

        return eligibleCards[random.Next(eligibleCards.Count)];
    }

    public CardSO GetRandomCard(Factions civilization)
    {
        System.Random random = new System.Random();
        var eligibleCards = CardDictionary.Values
        .Where(card => card.Faction == civilization && card.GetType() != typeof(MythCardSO))
        .ToList();

        return eligibleCards[random.Next(eligibleCards.Count)];
    }

    public List<CardSO> GetAllMyths(Factions civilization)
    {
        System.Random random = new System.Random();
        var eligibleCards = CardDictionary.Values
        .Where(card => card.Faction == civilization && card.GetType() == typeof(MythCardSO))
        .ToList();

        return eligibleCards;
    }

    //this is horrible, but for the scope of the project, i dont care too much : )
    public List<CardSO> GetPackContents(Factions? civilization)
    {
        var eligibleCards = CardDictionary.Values
        .Where(card => (civilization == null || card.Faction == civilization) && card.GetType() != typeof(MythCardSO))
        .ToList();

        if (!eligibleCards.Any())
        {
            return null;
        }

        System.Random random = new System.Random();
        List<CardSO> packContents = new List<CardSO>();

        for (int i = 0; i < 5; i++)
        {
            var randomCard = eligibleCards[random.Next(eligibleCards.Count)];
            packContents.Add(randomCard);
        }

        return packContents;
    }

    public List<CardSO> GetAllMyths()
    {
        var eligibleCards = CardDictionary.Values
        .Where(card => card.GetType() == typeof(MythCardSO))
        .ToList();

        return eligibleCards;
    }
}
