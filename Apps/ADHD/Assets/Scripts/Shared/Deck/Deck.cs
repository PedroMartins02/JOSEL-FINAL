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
            //this.Myth = new MythCard(deckSO.Myth);
            this.Faction = deckSO.Faction;

            this.Cards = new List<Card>();

            InitializeCards(deckSO.Cards);
        }

        private void InitializeCards(List<CardSO> cardSOList)
        {
            foreach (object cardSO in cardSOList)
            {
                if (cardSO.GetType() == typeof(UnitCardSO))
                {
                    Cards.Add(new UnitCard((UnitCardSO)cardSO));
                }
                else if (cardSO.GetType() == typeof(LegendCardSO))
                {
                    Cards.Add(new LegendCard((LegendCardSO)cardSO));
                }
                else if (cardSO.GetType() == typeof(BattleTacticCardSO))
                {
                    Cards.Add(new BattleTacticCard((BattleTacticCardSO)cardSO));
                }
            }
        }
    }
}