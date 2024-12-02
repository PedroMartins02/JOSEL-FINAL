using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }

    private NetworkVariable<int> _currentTurn = new NetworkVariable<int>();
    private NetworkVariable<int> _timeLeft = new NetworkVariable<int>();
    private NetworkVariable<int> _currentPlayerIdx = new NetworkVariable<int>();
    public NetworkVariable<ulong> CurrentPlayer = new NetworkVariable<ulong>();
    private NetworkList<ulong> _registeredPlayers;

    [SerializeField] private int _turnTime = 60;
    [SerializeField] private TextMeshProUGUI _timerLabel;
    [SerializeField] private Button _skipButton;

    private Coroutine _timerCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _registeredPlayers = new NetworkList<ulong>();
    }

    public override void OnDestroy()
    {
        _registeredPlayers?.Dispose();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SetupTurnManager;
        }
        else
        {
        }

        _timeLeft.OnValueChanged += UpdateTimerLabel;
        CurrentPlayer.OnValueChanged += UpdateButtonState;
    }

    private void SetupTurnManager(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientID in clientsCompleted)
        {
            if (_registeredPlayers.Contains(clientID)) return;

            _registeredPlayers.Add(clientID);

            if (clientID == NetworkManager.Singleton.LocalClientId)
            {
                _currentPlayerIdx.Value = _registeredPlayers.IndexOf(clientID);
                CurrentPlayer.Value = clientID;
            }
        }

        HandleSkipButtonRpc();
    }

    private void UpdateTimerLabel(int previous, int current)
    {
        _timerLabel.text = current.ToString();
    }

    private void OnTurnTimeout()
    {
        NextTurn();
    }

    [Rpc(SendTo.Server)]
    public void StartTurnRpc()
    {
        _currentTurn.Value++;

        StartTimerRpc();
        HandleSkipButtonRpc();

        EventManager.TriggerEvent(GameEventsEnum.TurnStarted, CurrentPlayer.Value);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HandleSkipButtonRpc()
    {
        UpdateButtonState();
    }

    private void UpdateButtonState(ulong previous = 0, ulong current = 1)
    {
        if (CurrentPlayer.Value == NetworkManager.Singleton.LocalClientId)
        {
            _skipButton.gameObject.SetActive(true);
        }
        else
        {
            _skipButton.gameObject.SetActive(false);
        }
    }

    public void NextTurn()
    {
        NextTurnRpc();
    }

    [Rpc(SendTo.Server)]
    public void NextTurnRpc()
    {
        StopTimerRpc();

        EventManager.TriggerEvent(GameEventsEnum.TurnEnded, CurrentPlayer.Value);

        _currentPlayerIdx.Value += 1;

        if (_currentPlayerIdx.Value >= _registeredPlayers.Count)
            _currentPlayerIdx.Value = 0;

        CurrentPlayer.Value = _registeredPlayers[_currentPlayerIdx.Value];

        StartTurnRpc();
    }

    [Rpc(SendTo.Server)]
    public void StartTimerRpc() 
    {
        _timerCoroutine = StartCoroutine(StartTimerCoroutine());
    }

    [Rpc(SendTo.Server)]
    public void StopTimerRpc()
    {
        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);

        _timerCoroutine = null;
    }

    private IEnumerator StartTimerCoroutine()
    {
        float timerFloatValue = _turnTime;

        while (timerFloatValue > 0) 
        {
            timerFloatValue -= Time.deltaTime;

            _timeLeft.Value = (int)Mathf.Ceil(timerFloatValue);

            yield return null;
        }

        _timeLeft.Value = 0;
        OnTurnTimeout();
    }

    public bool IsCurrentPlayer(ulong playerId) => playerId == CurrentPlayer.Value;

    public int CurrentTurn => _currentTurn.Value;
}
