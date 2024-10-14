using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    public class Deck
    {
        public string Name { get; private set; }
        public Card Myth { get; private set; }
        public List<Card> Cards { get; private set; }
        public Factions Faction { get; private set; }

        public Deck(DeckSO deckSO)
        {
            this.Name = deckSO.Name;
            this.Myth = deckSO.Myth;
            this.Cards = deckSO.Cards;
            this.Faction = deckSO.Faction;
        }
    }
}