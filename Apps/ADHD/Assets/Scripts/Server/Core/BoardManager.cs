using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic
{
    public class BoardManager
    {
        private static BoardManager instance = null;

        public static BoardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BoardManager();
                }
                return instance;
            }
        }

        private int numberOfAllowedCards = 4;
        private Dictionary<ulong, List<ICard>> playerCards = new();

        private BoardManager()
        {
            numberOfAllowedCards = 4;
            playerCards = new();
        }

        public void ChangeNumberOfAllowedCards(int allowedCards)
        {
            if (allowedCards <= 0)
                return; // TODO: Should throw an error

            numberOfAllowedCards = allowedCards;
        }

        public bool CanPlayAnotherCard(ulong playerId)
        {
            playerCards.TryGetValue(playerId, out var cardList);

            return cardList == null || cardList.Count < numberOfAllowedCards;
        }

        public void AddCardToBoard(ulong playerId, ICard card)
        {
            if (!CanPlayAnotherCard(playerId)) return; // TODO: Should throw an error

            playerCards.TryGetValue(playerId, out var cardList);

            cardList ??= new();
            cardList.Add(card);

            playerCards.Add(playerId, cardList);
        }
    }
}