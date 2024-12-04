using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Game.Logic
{
    public class CardManager
    {
        private static CardManager instance = null;

        public static CardManager Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = new CardManager();
                }
                return instance; 
            } 
        }

        private Dictionary<int, ICard> cards = new();
        private int nextAvailableId = 0;

        private CardManager() 
        {
            cards = new();
        }

        public void Clear()
        {
            cards = new();
            nextAvailableId = 0;
        }

        public int RegisterCard(ICard card)
        {
            int cardId = nextAvailableId;
            nextAvailableId++;

            cards.Add(cardId, card);

            return cardId;
        }

        public bool UpdateCard(ICard card)
        {
            if (!cards.ContainsKey(card.Data.GameID))
                return false;

            cards[card.Data.GameID] = card;
            return true;
        }

        public ICard CreateCard(ulong cardId)
        {

            return null;
        }

        public void RemoveCard(int cardId)
        {
            cards.Remove(cardId);
        }

        public ICard GetCardByGameId(int cardId)
        {
            cards.TryGetValue(cardId, out ICard card);
            return card;
        }

        public CardDataSnapshot GetCardSnapshot(int cardId)
        {
            cards.TryGetValue(cardId, out ICard card);

            if (card == null)
                return new CardDataSnapshot { };

            return card.GetDataSnapshot();
        }
    }
}
