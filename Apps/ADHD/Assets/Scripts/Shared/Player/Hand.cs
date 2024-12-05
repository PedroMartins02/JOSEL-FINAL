using System.Collections;
using System.Collections.Generic;
using Game.Logic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace GameModel
{
    public class Hand
    {
        private List<ICard> cards;
        public int MaxHandSize { get; private set; }

        public Hand(int maxHandSize = 10) 
        {
            cards = new List<ICard>();

            MaxHandSize = maxHandSize;
        }

        public bool AddCard(ICard card)
        {
            if (cards.Count >= MaxHandSize)
            {
                return false;
            }

            cards.Add(card);
            return true;
        }

        public void RemoveCard(ICard card)
        {
            cards.Remove(card);
        }

        public bool Contains(ICard card)
        {
            return cards.Contains(card);
        }

        public bool Contains(int cardGameID)
        {
            return cards.Find(card => card.Data.GameID == cardGameID) != null;
        }

        public ICard GetCard(int cardGameID)
        {
            return cards.Find(card => card.Data.GameID == cardGameID);
        }

        public IReadOnlyList<ICard> GetCards() => cards.AsReadOnly();
    }
}