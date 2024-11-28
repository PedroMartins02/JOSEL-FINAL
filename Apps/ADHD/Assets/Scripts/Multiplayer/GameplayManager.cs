using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions.UI;
using GameModel;
using Unity.Netcode;
using UnityEngine.SceneManagement;

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
    public event EventHandler OnGameStart;


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

    private void Update()
    { 
        //Debug.Log(PlayerManager.Instance.PlayerCount);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetupGame;
        }

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
    }

    public void StartGame()
    {
        if (IsServer)
        {

            StartGameClientRpc();
        }
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        CurrentGameState.Value = GameState.Playing;

        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    public void BroadcastActionExecuted(ActionData actionData)
    {
        if (IsServer)
        {
            NotifyActionExecutedClientRpc(actionData);
        }
    }

    [ClientRpc]
    private void NotifyActionExecutedClientRpc(ActionData actionData)
    {
        IUIAction uiAction = UIActionFactory.CreateUIAction(actionData);

        UIActionQueueManager.Instance.EnqueueAction(uiAction);
    }

    private void GameOver()
    {

    }
}
