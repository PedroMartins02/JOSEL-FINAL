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
            


        }

        private bool IsValidPlayCardRequest(Player player, ICard card)
        {
            return player != null
                && card != null
                && player.Hand.Contains(card); // TODO: Check if player turn
        }
    }
}