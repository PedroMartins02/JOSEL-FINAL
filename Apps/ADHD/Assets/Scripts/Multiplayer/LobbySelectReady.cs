using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbySelectReady : NetworkBehaviour
{
    public static LobbySelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void ChangePlayerReady()
    {
        ChangePlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        if (playerReadyDictionary.ContainsKey(senderClientId))
            playerReadyDictionary[senderClientId] = !playerReadyDictionary[senderClientId];
        else
            playerReadyDictionary[senderClientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);

        bool allClientsReady = true;
        if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
                {
                    // This player is NOT ready
                    allClientsReady = false;
                    break;
                }
            }
        }
        else
            allClientsReady = false;

        if (allClientsReady)
        {
            LobbyManager.Instance.DeleteLobby();
            SceneLoader.LoadNetwork(SceneLoader.Scene.Game);
        }
    }

    /**
     * ClientRPC to tell the client who is ready
     */
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        if (!IsServer)
        {
            if (playerReadyDictionary.ContainsKey(clientId))
                playerReadyDictionary[clientId] = !playerReadyDictionary[clientId];
            else
                playerReadyDictionary[clientId] = true;

            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /**
     * Method to return if given player is ready
     */
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
