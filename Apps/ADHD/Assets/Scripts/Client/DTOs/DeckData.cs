using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GameModel;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class DeckData
{
    public string Name { get; set; }
    public int CardBackId { get; set; }
    public string MythId { get; set; }
    public List<string> CardList { get; set; }

    // Parameterless constructor for JSON deserialization
    public DeckData()
    {
        Name = "New Deck";
        CardBackId = 0;
        MythId = null;
        CardList = new List<string>();
    }

    public DeckData(DeckSO deckSO) { 
        Name = deckSO.Name;
        CardBackId = 0;
        MythId = deckSO.Myth.Id;
        CardList = new List<string>();
        
        foreach (CardSO cardSO in deckSO.Cards)
        {
            CardList.Add(cardSO.Id);
        }
    }

    public DeckData(DeckSO deck, int cardBackId)
    {
        Name = deck.Name;
        CardBackId = cardBackId;
        MythId = deck.Myth.Id;
        CardList = new List<string>();
        
        foreach (CardSO cardSO in deck.Cards)
        {
            CardList.Add(cardSO.Id);
        }
    }

    public static string SerializeDeckData(DeckData deckData)
    {
        StringBuilder result = new StringBuilder();

        result.Append(deckData.CardBackId.ToString());
        result.Append('|');
        result.Append(deckData.MythId);
        result.Append('|');
        result.Append(string.Join("|", deckData.CardList.Select(cardId => cardId)));

        return result.ToString();
    }

    public static DeckData DeserializeDeckData(string serializedDeckData)
    {
        string[] data = serializedDeckData.Split('|');
        DeckData deckData = new();
        List<string> cardList = new(data);

        deckData.CardBackId = int.Parse(data[0]);
        deckData.MythId = data[1];

        cardList.RemoveRange(0, 2);

        deckData.CardList = cardList;

        return deckData;
    }
}
