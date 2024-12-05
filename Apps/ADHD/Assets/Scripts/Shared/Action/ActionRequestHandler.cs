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

        [Rpc(SendTo.Server)]
        public void HandlePlayCardRequestServerRpc(int cardGameID, RpcParams rpcParams = default)
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

        [Rpc(SendTo.Server)]
        public void HandleDrawCardRequestServerRpc(int numberOfCardsToDraw, ulong clientID, RpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.DrawCard,
                PlayerId = clientID,
                NumberOfCardsDrawn = numberOfCardsToDraw
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }

        [Rpc(SendTo.Server)]
        public void HandleAttackCardRequestServerRpc(int attackingCardGameID, int targetCardGameID, ulong clientID, RpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.AttackCard,
                PlayerId = clientID,
                CardGameID = attackingCardGameID,
                TargetCardGameID = targetCardGameID
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }

        [Rpc(SendTo.Server)]
        public void HandleAttackMythRequestServerRpc(ulong clientID, ulong targetPlayerID, int attackingCardGameID, RpcParams rpcParams = default)
        {
            ActionData actionData = new ActionData
            {
                ActionType = ActionType.AttackMyth,
                PlayerId = clientID,
                TargetPlayerId = targetPlayerID,
                CardGameID = attackingCardGameID,
            };

            IAction action = ActionFactory.CreateAction(actionData);
            ActionQueueManager.Instance.AddAction(action);
        }
    }
}