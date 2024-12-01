using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }

    private NetworkVariable<int> _timeLeft = new NetworkVariable<int>();
    private NetworkVariable<int> _currentPlayerIdx = new NetworkVariable<int>();
    [NonSerialized] public NetworkVariable<ulong> CurrentPlayer = new NetworkVariable<ulong>();
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
            _currentPlayerIdx.Value = 0;
        }
        else
        {

        }

        _timeLeft.OnValueChanged += UpdateTimerLabel;
    }

    [Rpc(SendTo.Server)]
    public void RegisterPlayerRpc(ulong PlayerID, RpcParams rpcParams = default)
    {
        if (_registeredPlayers.Contains(PlayerID)) return;

        _registeredPlayers.Add(PlayerID);


        // If is host
        if (PlayerID == OwnerClientId)
        {
            _currentPlayerIdx.Value = _registeredPlayers.IndexOf(PlayerID);
            CurrentPlayer.Value = PlayerID;
        }
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
        HandleSkipButtonRpc();
        StartTimerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HandleSkipButtonRpc()
    {
        if (CurrentPlayer.Value == OwnerClientId)
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
}
