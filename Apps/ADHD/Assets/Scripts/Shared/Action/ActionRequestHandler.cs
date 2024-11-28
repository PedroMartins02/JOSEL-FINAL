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
        public void HandlePlayCardRequestServerRpc(ulong networkObjectCardId, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.PlayCard,
                PlayerId = rpcParams.Receive.SenderClientId,
                NetworkObjectCardId = networkObjectCardId
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }

        [ServerRpc]
        public void HandleDrawCardRequestServerRpc(int numberOfCardsDrawn, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.DrawCard,
                PlayerId = rpcParams.Receive.SenderClientId,
                NumberOfCardsDrawn = numberOfCardsDrawn
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }
    }
}