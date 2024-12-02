using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Logic;
using UnityEngine;

namespace GameModel
{
    public class Player
    {
        public readonly PlayerGameData playerData;
        public int CurrentHealth;
        public int CurrentBlessings;
        public int CurrentMaxBlessings;
        public readonly MythCard MythCard;
        public readonly Hand Hand;
        public readonly Deck Deck;

        public Player(PlayerGameData playerData)
        {
            this.playerData = playerData;

            this.CurrentHealth = playerData.Health;
            this.CurrentBlessings = playerData.Blessings;
            this.CurrentMaxBlessings = playerData.Blessings;
            this.Deck = playerData.Deck;
            this.MythCard = playerData.MythCard;

            this.Hand = new Hand();
        }

        public void PlayCard(int cardGameID)
        {
            if (!HasCardInHand(cardGameID)) return;

            ICard card = Hand.GetCard(cardGameID);

            if (card.CurrentBlessingsCost > CurrentBlessings) return;

            UseBlessings(card.CurrentBlessingsCost);

            HandlePlayCard(card);
        }

        public void PlayCard(ICard card)
        {
            if (!HasCardInHand(card)) return;

            HandlePlayCard(card);
        }

        private void HandlePlayCard(ICard card)
        {
            Hand.RemoveCard(card);
            card.StateMachine.SetState(CardStateType.InPlay);

            CardManager.Instance.UpdateCard(card);
        }

        public bool HasCardInHand(ICard card)
        {
            return Hand.Contains(card);
        }

        public bool HasCardInHand(int cardGameID)
        {
            return Hand.Contains(cardGameID);
        }

        public bool HasCardOnField(ICard card)
        {
            return BoardManager.Instance.HasCardOnBoard(playerData.ClientId, card);
        }

        public void UseBlessings(int amount)
        {
            CurrentBlessings -= amount;

            if (CurrentBlessings < 0)
                CurrentBlessings = 0;
        }

        public void RestockBlessings()
        {
            CurrentBlessings = CurrentMaxBlessings;
        }

        public void GiveBlessings(int amount)
        {
            CurrentBlessings += amount;
        }

        public void RaiseMaxBlessings(int amount = 1)
        {
            CurrentMaxBlessings += amount;
        }
    }
}
