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

        [ServerRpc(RequireOwnership = false)]
        public void HandlePlayCardRequestServerRpc(int cardGameID, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.PlayCard,
                PlayerId = rpcParams.Receive.SenderClientId,
                CardGameID = cardGameID
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }

        [ServerRpc(RequireOwnership = false)]
        public void HandleDrawCardRequestServerRpc(int numberOfCardsToDraw, ServerRpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.DrawCard,
                PlayerId = rpcParams.Receive.SenderClientId,
                NumberOfCardsDrawn = numberOfCardsToDraw
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }
    }
}