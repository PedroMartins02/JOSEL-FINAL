using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuickGameController : MonoBehaviour
{
    [SerializeField] private GameObject queue;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button cancelQueueBtn;
    
    private float elapsedTime;
    private bool isTimerRunning;
    private bool isJoiningMatch;

    private void Awake()
    {
        queue.SetActive(false);

        cancelQueueBtn.onClick.AddListener(() =>
        {
            if (LobbyManager.Instance.IsLobbyHost() && !isJoiningMatch)
            {
                LobbyManager.Instance.DeleteLobby();
                StopTimer();
                queue.SetActive(false);
            }
        });
    }

    private void Start()
    {
        LobbyManager.Instance.OnQuickMatchStarted += LobbyManager_OnQuickMatchStarted;
        LobbyManager.Instance.OnQuickMatchFailed += LobbyManager_OnQuickMatchFailed;
        LobbyManager.Instance.OnJoinedLobbyUpdate += LobbyManager_OnJoinedLobbyUpdate;

        isTimerRunning = false;
        isJoiningMatch = false;

        cancelQueueBtn.enabled = true;

    }

    private void LobbyManager_OnJoinedLobbyUpdate(object sender, System.EventArgs e)
    {
        Debug.Log("LOBBY QUICK MATCH PLAYERS: " + LobbyManager.Instance.GetLobby().Players.Count);
        if (LobbyManager.Instance.IsLobbyHost() && LobbyManager.Instance.GetLobby().Players.Count > 1)
        {
            StopTimer();
            timerText.text = "MATCH FOUND";
            cancelQueueBtn.enabled = false;
            isJoiningMatch = true;
            cancelQueueBtn.gameObject.SetActive(false);
            StartCoroutine(WaitCoroutine());
        }
        else if (LobbyManager.Instance.GetLobby().Players.Count > 1)
        {
            StopTimer();
            timerText.text = "MATCH FOUND";
            cancelQueueBtn.enabled = false;
            isJoiningMatch = true;
            cancelQueueBtn.gameObject.SetActive(false);
            LobbyManager.Instance.ClearJoinedLobby();
        }
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(5);

        LobbyManager.Instance.JoinQuickMatchGame();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            UpdateTimerDisplay();
        }
    }

    private void LobbyManager_OnQuickMatchStarted(object sender, System.EventArgs e)
    {
        StartTimer();
    }

    private void LobbyManager_OnQuickMatchFailed(object sender, System.EventArgs e)
    {
        StopTimer();
    }

    private void StartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        queue.SetActive(true);

        UpdateTimerDisplay();
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void FindGame()
    {
        LobbyManager.Instance.QuickMatchLobby();
    }

    private void OnDestroy()
    {
        if(LobbyManager.Instance != null) 
            LobbyManager.Instance.LeaveLobby();

        LobbyManager.Instance.OnQuickMatchStarted -= LobbyManager_OnQuickMatchStarted;
        LobbyManager.Instance.OnQuickMatchFailed -= LobbyManager_OnQuickMatchFailed;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= LobbyManager_OnJoinedLobbyUpdate;
    }
}
