using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public class UIAttackCardAction : IUIAction
    {
        private ActionData _actionData;

        public UIAttackCardAction(ActionData actionData)
        {
            _actionData = actionData;
        }

        public IEnumerator Execute()
        {
            // Update the Client Interface and stuff

            bool isMyAction = MultiplayerManager.Instance.IsPlayerInstanceHost(_actionData.PlayerId);



            yield return null;
        }
    }
}