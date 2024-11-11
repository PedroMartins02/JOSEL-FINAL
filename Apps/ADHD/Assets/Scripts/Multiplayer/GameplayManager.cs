using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance { get; private set; }

    // Events
    public event EventHandler OnGameFinishedLoading;

    // Variables
    private NetworkVariable<float> gameTimer;


    private void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Assign to event for when a player disconnects
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            // Event for when the game scene is loaded
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        throw new NotImplementedException();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // Setup the data for the game managers, in this case the game rules
        SetInitialSettingsClientRpc();

        // We need to set player data to the UI with events
        throw new NotImplementedException();
    }

    [ClientRpc]
    private void SetInitialSettingsClientRpc()
    {
        
    }
}
