using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Logic;
using Game.Logic.Modifiers;
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

        public Deck(string name, List<string> cardIdList, Elements playerWeather, Elements playerTime)
        {
            this.Name = name;

            this.cards = new();

            InitializeCards(cardIdList, playerWeather, playerTime);
            Shuffle();

            this.Faction = cards.Peek().Data.Faction;
        }

        private void InitializeCards(List<string> cardIdList, Elements playerWeather, Elements playerTime)
        {
            // TODO: There should be a Card Factory

            foreach (string cardID in cardIdList)
            {
                ICard card = CardDatabase.Singleton.GetCardOfId(cardID);

                if (card.GetType() == typeof(UnitCard))
                {
                    UnitCard unitCard = new UnitCard((UnitCardData)card.Data);

                    if (unitCard.Data.Element == playerWeather)
                    {
                        WeatherModifier weatherModifier = new WeatherModifier();
                        unitCard.ApplyModifier(weatherModifier);
                    }

                    cards.Push(unitCard);
                }
                else if (card.GetType() == typeof(LegendCard))
                {
                    LegendCard legendCard = new LegendCard((LegendCardData)card.Data);

                    if (legendCard.Data.Element == playerTime)
                    {
                        WeatherModifier weatherModifier = new WeatherModifier();
                        legendCard.ApplyModifier(weatherModifier);
                    }

                    cards.Push(legendCard);
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