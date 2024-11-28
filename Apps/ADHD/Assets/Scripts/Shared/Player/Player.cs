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
        }

        public void PlayCard(ICard card)
        {
            if (!HasCard(card)) return;

            Hand.RemoveCard(card);

            card.StateMachine.SetState(CardStateType.InPlay);
        }

        public bool HasCard(ICard card)
        {
            return Hand.Contains(card);
        }
    }
}
