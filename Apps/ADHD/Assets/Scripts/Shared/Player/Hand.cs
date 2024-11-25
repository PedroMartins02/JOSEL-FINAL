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

        public Hand(int maxHandSize) 
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

        public IReadOnlyList<ICard> GetCards() => cards.AsReadOnly();
    }
}