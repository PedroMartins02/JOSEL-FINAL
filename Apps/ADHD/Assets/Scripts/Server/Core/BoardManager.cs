using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using GameModel;
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

            EventManager.Subscribe(GameEventsEnum.CardDied, OnCardDiedEvent);
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

        public void AddCardToBoard(ulong playerId, int cardGameID)
        {
            if (!CanPlayAnotherCard(playerId)) return; // TODO: Should throw an error

            ICard card = CardManager.Instance.GetCardByGameId(cardGameID);

            AddCardToBoard(playerId, card);
        }

        public void AddCardToBoard(ulong playerId, ICard card)
        {
            if (!CanPlayAnotherCard(playerId)) return; // TODO: Should throw an error

            playerCards.TryGetValue(playerId, out var cardList);

            cardList ??= new();
            cardList.Add(card);

            playerCards[playerId] = cardList;
        }

        public bool HasCardOnBoard(ulong playerId, ICard card)
        {
            playerCards.TryGetValue(playerId, out var cardList);

            if (cardList == null) return false;

            return cardList.Contains(card);
        }

        public void RemoveCardFromBoard(int cardID, ICard card)
        {
            foreach (ulong playerId in playerCards.Keys)
            {
                playerCards.TryGetValue(playerId, out var cardList);

                if (cardList == null) continue;

                if (cardList.Contains(card))
                {
                    cardList.Remove(card);
                    playerCards[playerId] = cardList;
                    return;
                }
            }
        }

        private void OnCardDiedEvent(object args)
        {
            if (!(args is ICard))
                return;

            ICard card = args as ICard;

            RemoveCardFromBoard(card.Data.GameID, card);
        }
    }
}