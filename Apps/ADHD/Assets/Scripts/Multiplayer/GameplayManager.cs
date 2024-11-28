using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.Actions.UI;
using GameModel;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LobbyManager;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance { get; private set; }

    // Game State
    public enum GameState
    {
        WaitingForPlayers, // Waiting for all players to load
        Playing,
        GameOver
    }

    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    // Events
    public event EventHandler<GameStateEventArgs> OnCurrentGameStateChanged;

    public class GameStateEventArgs : EventArgs
    {
        public GameState gameState;
    }


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

        if (LobbyManager.Instance != null)
        {
            Destroy(LobbyManager.Instance.gameObject);
        }
    }

    private void Start()
    {

    }

    
    private void GameplayManager_GameStateChange(GameState previousValue, GameState newValue)
    {
        Debug.Log("WRRAAAAA 2");

        OnCurrentGameStateChanged?.Invoke(this, new GameStateEventArgs { gameState = newValue });
    }

    public override void OnNetworkSpawn()
    {
        CurrentGameState.OnValueChanged += GameplayManager_GameStateChange;

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetupGame;
        }

        base.OnNetworkSpawn();
    }

    private void SetupGame(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (clientsTimedOut.Count > 0)
            GameOver();

        List<GameRule> rules = MultiplayerManager.Instance.GetLobbyGameRules();
        GameRulesManager.Instance.IntializeGameRules(rules);

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            MP_PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(clientId);

            PlayerManager.Instance.CreatePlayer(clientId, playerData);
        }

        Debug.Log("WRRAAAAA 1");
        CurrentGameState.Value = GameState.Playing;

        StartGameClientRpc(GameRulesManager.Instance.GetIntRuleValue(RuleTarget.StartingHandSize));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameClientRpc(int cardsToDraw)
    {
        StartGame();

        ActionRequestHandler.Instance.HandleDrawCardRequestServerRpc(cardsToDraw);
    }

    private void StartGame()
    {
        
    }

    public void BroadcastActionExecuted(ActionData actionData)
    {
        if (IsServer)
        {
            NotifyActionExecutedClientRpc(actionData);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyActionExecutedClientRpc(ActionData actionData)
    {
        IUIAction uiAction = UIActionFactory.CreateUIAction(actionData);

        UIActionQueueManager.Instance.EnqueueAction(uiAction);
    }

    public GameState GetCurrentGameState()
    {
        return this.CurrentGameState.Value;
    }

    private void GameOver()
    {

    }
}
