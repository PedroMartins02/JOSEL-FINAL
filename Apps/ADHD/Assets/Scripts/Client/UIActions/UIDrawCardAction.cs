using System.Collections;
using System.Collections.Generic;
using Game.Data;
using GameCore.Events;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIDrawCardAction : IUIAction
    {
        private CardDataSnapshot cardData;
        private ulong playerId;

        public UIDrawCardAction(CardDataSnapshot cardData, ulong playerId)
        {
            this.cardData = cardData;
            this.playerId = playerId;
        }

        public IEnumerator Execute()
        {
            EventManager.TriggerEvent(GameEventsEnum.CardDrawn, new CardDrawnEventArgs { CardData = cardData, PlayerID = playerId });

            // Do Generic stuff if necessary here (or before the if statement)

            yield return null;
        }
    }
}