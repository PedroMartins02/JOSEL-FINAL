using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Logic;
using GameCore.Events;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace GameModel
{
    public class Deck
    {
        public string Name { get; private set; }
        public Card Myth { get; private set; }
        public Factions Faction { get; private set; }

        private Stack<ICard> cards;

        public Deck(DeckSO deckSO)
        {
            this.Name = deckSO.Name;
            //this.Myth = new MythCard(deckSO.Myth);
            this.Faction = deckSO.Faction;

            this.cards = new();

            InitializeCards(deckSO.Cards);
            Shuffle();
        }

        private void InitializeCards(List<CardSO> cardSOList)
        {
            foreach (CardSO cardSO in cardSOList)
            {
                if (cardSO.GetType() == typeof(UnitCardSO))
                {
                    cards.Push(new UnitCard((UnitCardData)cardSO.CardData));
                }
                else if (cardSO.GetType() == typeof(LegendCardSO))
                {
                    cards.Push(new LegendCard((LegendCardData)cardSO.CardData));
                }
                else if (cardSO.GetType() == typeof(BattleTacticCardSO))
                {
                    cards.Push(new BattleTacticCard((BattleTacticCardData)cardSO.CardData));
                }
            }
        }

        public void Shuffle()
        {
            var cardList = cards.ToList();
            var random = new System.Random();

            cardList = cardList.OrderBy(c => random.Next()).ToList();

            cards = new Stack<ICard>(cardList);
        }

        public ICard DrawCard()
        {
            if (cards.Count > 0)
            {
                ICard card = cards.Pop();
                EventManager.TriggerEvent(GameEventsEnum.CardDrawn, card);

                return card;
            }

            EventManager.TriggerEvent(GameEventsEnum.DeckExhausted);
            return null;
        }

        public int CardsRemaining => cards.Count;
    }
}