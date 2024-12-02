using System;
using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.Actions.UI;
using GameCore.Events;
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
        OnCurrentGameStateChanged?.Invoke(this, new GameStateEventArgs { gameState = newValue });
    }

    public override void OnNetworkSpawn()
    {
        CurrentGameState.OnValueChanged += GameplayManager_GameStateChange;

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetupGame;

            EventManager.Subscribe(GameEventsEnum.TurnStarted, HandleTurnStarted);
            EventManager.Subscribe(GameEventsEnum.TurnEnded, HandleTurnEnded);
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

        CurrentGameState.Value = GameState.Playing;

        StartGameClientRpc(GameRulesManager.Instance.GetIntRuleValue(RuleTarget.StartingHandSize));
        TurnManager.Instance.StartTurnRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameClientRpc(int cardsToDraw)
    {
        ActionRequestHandler.Instance.HandleDrawCardRequestServerRpc(cardsToDraw, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    public void BroadcastActionExecutedRpc(ActionData actionData)
    {
        NotifyActionExecutedClientRpc(actionData);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void NotifyActionExecutedClientRpc(ActionData actionData)
    {
        IUIAction uiAction = UIActionFactory.CreateUIAction(actionData);

        UIActionQueueManager.Instance.EnqueueAction(uiAction);
    }

    [Rpc(SendTo.Server)]
    public void BroadcasUpdatePlayerInfoRpc()
    {
        EventManager.TriggerEvent(GameEventsEnum.PlayerInfoChanged);
    }

    public GameState GetCurrentGameState()
    {
        return this.CurrentGameState.Value;
    }

    private void HandleTurnStarted(object args)
    {
        Player player = HandleTurnEventArgs(args);

        if (player == null) return;

        ulong playerID = (ulong)args;

        if (TurnManager.Instance.CurrentTurn > 2)
        {
            player.RaiseMaxBlessings(GameRulesManager.Instance.GetIntRuleValue(RuleTarget.BlessingPerTurn));
            player.RestockBlessings();
        }

        ActionRequestHandler.Instance.HandleDrawCardRequestServerRpc(GameRulesManager.Instance.GetIntRuleValue(RuleTarget.CardsDrawnPerTurn), playerID);

        EventManager.TriggerEvent(GameEventsEnum.PlayerInfoChanged);
    }

    private void HandleTurnEnded(object args)
    {
        Player player = HandleTurnEventArgs(args);

        if (player == null) return;

    }

    private Player HandleTurnEventArgs(object args)
    {
        if ((args.GetType() != typeof(ulong)) || !IsServer) return null;

        ulong playerID = (ulong)args;

        Player player = PlayerManager.Instance.GetPlayerByClientId(playerID);

        return player;
    }

    private void GameOver()
    {

    }
}
