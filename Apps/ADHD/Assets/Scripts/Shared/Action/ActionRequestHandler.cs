using System.Collections;
using System.Collections.Generic;
using GameModel;
using Unity.Netcode;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class ActionRequestHandler : NetworkBehaviour
    {
        public static ActionRequestHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [ServerRpc]
        public void HandlePlayCardRequestServerRpc(ulong clientId, ulong cardId, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.PlayCard,
                PlayerId = clientId,
                NetworkObjectCardId = cardId
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }

        [ServerRpc]
        public void HandleDrawCardRequestServerRpc(ulong clientId, int numberOfCardsDrawn, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.DrawCard,
                PlayerId = clientId,
                NumberOfCardsDrawn = numberOfCardsDrawn
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }
    }
}