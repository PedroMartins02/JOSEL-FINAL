using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public CardSO[] AllCards { get; private set; } = new CardSO[0];

    private void Start()
    {
        LoadCards();
    }

    private void LoadCards()
    {
        AllCards = Resources.LoadAll<CardSO>("ScriptableObjects/Cards");
        Debug.Log($"Loaded {AllCards.Count()} cards");
    }

    public Card GetCardOfId(string cardId)
    {
        CardSO cardSO = AllCards.Where(card => card.Id.Equals(cardId)).FirstOrDefault();

        if (cardSO == null)
        {
            return null;
        }

        if (cardSO.GetType() == typeof(UnitCardSO))
        {
            return new UnitCard((UnitCardSO)cardSO);
        }

        if (cardSO.GetType() == typeof(BattleTacticCardSO))
        {
            return new BattleTacticCard((BattleTacticCardSO)cardSO);
        }

        if (cardSO.GetType() == typeof(LegendCardSO))
        {
            return new LegendCard((LegendCardSO)cardSO);
        }

        if (cardSO.GetType() == typeof(MythCardSO))
        {
            return new MythCard((MythCardSO)cardSO);
        }

        return null;
    }


    public CardSO GetCardSoOfId(string cardId)
    {
        return AllCards.Where(card => card.Id.Equals(cardId)).FirstOrDefault();
    }
}
