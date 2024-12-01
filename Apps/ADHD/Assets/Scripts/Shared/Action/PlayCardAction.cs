using UnityEngine;
using GameModel;
using Game.Data;

namespace Game.Logic.Actions
{
    public class PlayCardAction : IAction
    {
        private Player player;
        private int cardGameID;

        public PlayCardAction(Player player, int cardGameID)
        {
            this.player = player;
            this.cardGameID = cardGameID;
        }

        public bool IsLegal()
        {
            CardDataSnapshot snapshot = CardManager.Instance.GetCardSnapshot(cardGameID);

            return BoardManager.Instance.CanPlayAnotherCard(player.playerData.ClientId)
                && player.HasCard(cardGameID)
                && player.CurrentBlessings >= snapshot.CurrentBlessings;
        }

        public void Execute()
        {
            player.PlayCard(cardGameID);

            BoardManager.Instance.AddCardToBoard(player.playerData.ClientId, cardGameID);

            CardDataSnapshot snapshot = CardManager.Instance.GetCardSnapshot(cardGameID);

            ActionData actionData = new ActionData
            {
                ActionType = ActionType.PlayCard,
                PlayerId = player.playerData.ClientId,
                CardGameID = cardGameID,
            };

            GameplayManager.Instance.BroadcastActionExecutedRpc(actionData);
            GameplayManager.Instance.BroadcasUpdatePlayerInfoRpc();
        }
    }
}
