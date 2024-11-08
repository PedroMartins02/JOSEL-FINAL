using System.Collections;
using System.Collections.Generic;
using GameModel;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class DeckData
{
    public string Name { get; set; }
    public int CardBackId { get; set; }
    public string MythId { get; set; }
    public List<string> CardList { get; set; } = new List<string>();

    // Parameterless constructor for JSON deserialization
    public DeckData() { }

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
}
