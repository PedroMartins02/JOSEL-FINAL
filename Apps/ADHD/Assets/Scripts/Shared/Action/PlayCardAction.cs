using System;
using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class PlayCardAction : IAction
    {
        private Player player;
        private ICard card;

        public PlayCardAction(Player player, ICard card)
        {
            this.player = player;
            this.card = card;
        }

        public bool IsLegal()
        {
            return BoardManager.Instance.CanPlayAnotherCard(player.playerData.ClientId)
                && player.HasCard(card);
        }

        public void Execute()
        {
            player.PlayCard(card);

            BoardManager.Instance.AddCardToBoard(player.playerData.ClientId, card);

            ActionData actionData = new ActionData
            {
                ActionType = ActionType.PlayCard,
                PlayerId = player.playerData.ClientId,
                NetworkObjectCardId = card.NetworkId,
            };

            GameplayManager.Instance.BroadcastActionExecuted(actionData);
        }
    }
}
