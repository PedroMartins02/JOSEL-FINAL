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
        public readonly MythCard MythCard;
        public readonly Hand Hand;
        public readonly Deck Deck;

        public Player(PlayerGameData playerData)
        {
            this.playerData = playerData;

            this.CurrentHealth = playerData.Health;
            this.CurrentBlessings = playerData.Blessings;
            this.Deck = playerData.Deck;
            this.MythCard = playerData.MythCard;

            this.Hand = new Hand();
        }

        public void PlayCard(int cardGameID)
        {
            if (!HasCard(cardGameID)) return;

            ICard card = Hand.GetCard(cardGameID);

            HandlePlayCard(card);
        }

        public void PlayCard(ICard card)
        {
            if (!HasCard(card)) return;

            HandlePlayCard(card);
        }

        private void HandlePlayCard(ICard card)
        {
            Hand.RemoveCard(card);
            card.StateMachine.SetState(CardStateType.InPlay);

            CardManager.Instance.UpdateCard(card);
        }

        public bool HasCard(ICard card)
        {
            return Hand.Contains(card);
        }

        public bool HasCard(int cardGameID)
        {
            return Hand.Contains(cardGameID);
        }
    }
}
