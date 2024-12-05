using System.Collections;
using System.Collections.Generic;
using Game.Data;
using GameCore.Events;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIPlayCardAction : IUIAction
    {
        private int cardGameID;
        private ulong playerId;

        public UIPlayCardAction(int cardGameID, ulong playerId)
        {
            this.cardGameID = cardGameID;
            this.playerId = playerId;
        }

        public IEnumerator Execute()
        {
            EventManager.TriggerEvent(GameEventsEnum.CardPlayed, new CardPlayedEventArgs { CardGameID = cardGameID, PlayerID = playerId });

            // Do Generic stuff if necessary here (or before the if statement)

            yield return null;
        }
    }
}