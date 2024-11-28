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
        public Factions Faction { get; private set; }

        private Stack<ICard> cards;

        public Deck(string name, List<string> cardIdList)
        {
            this.Name = name;

            this.cards = new();

            InitializeCards(cardIdList);
            Shuffle();

            this.Faction = cards.Peek().Data.Faction;
        }

        private void InitializeCards(List<string> cardIdList)
        {
            foreach (string cardID in cardIdList)
            {
                ICard card = CardDatabase.Singleton.GetCardOfId(cardID);

                if (card.GetType() == typeof(UnitCard))
                {
                    cards.Push(new UnitCard((UnitCardData)card.Data));
                }
                else if (card.GetType() == typeof(LegendCard))
                {
                    cards.Push(new LegendCard((LegendCardData)card.Data));
                }
                else if (card.GetType() == typeof(BattleTacticCard))
                {
                    cards.Push(new BattleTacticCard((BattleTacticCardData)card.Data));
                }
            }
        }

        public void Shuffle()
        {
            var cardList = cards.ToList();
            var random = new System.Random();

            cardList = cardList.OrderBy(c => random.Next()).ToList();

            this.cards = new Stack<ICard>(cardList);
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