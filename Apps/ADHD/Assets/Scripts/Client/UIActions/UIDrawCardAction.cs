using System.Collections;
using System.Collections.Generic;
using GameModel;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIDrawCardAction : IUIAction
    {
        private string cardID;
        private ulong playerId;

        public UIDrawCardAction(string cardID, ulong playerId)
        {
            this.cardID = cardID;
            this.playerId = playerId;
        }

        public IEnumerator Execute()
        {
            bool isMyAction = MultiplayerManager.Instance.IsPlayerInstanceHost(playerId);

            if (isMyAction)
            {
                CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(cardID);

                // Update UI taking into account I played the card
            }
            else
            {
                // Update UI taking into account I didn't play the card
            }

            // Do Generic stuff if necessary here (or before the if statement)

            yield return null;
        }
    }
}