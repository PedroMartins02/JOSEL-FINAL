using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

        private Dictionary<ulong, ICard> cards = new Dictionary<ulong, ICard>();

        private CardManager() 
        {
            cards = new Dictionary<ulong, ICard>();
        }

        public void RegisterCard(ulong cardId, ICard card)
        {
            cards.Add(cardId, card);
        }

        public ICard CreateCard(ulong cardId)
        {

            return null;
        }

        public void RemoveCard(ulong cardId)
        {
            cards.Remove(cardId);
        }

        public ICard GetCardByNetworkId(ulong cardId)
        {
            cards.TryGetValue(cardId, out ICard card);
            return card;
        }
    }
}
