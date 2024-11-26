using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuickGameController : MonoBehaviour
{
    [SerializeField] private GameObject queue;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float elapsedTime;
    private bool isTimerRunning;

    private void Awake()
    {
        queue.SetActive(false);
    }

    private void Start()
    {
        LobbyManager.Instance.OnQuickMatchStarted += LobbyManager_OnQuickMatchStarted; ;
        LobbyManager.Instance.OnQuickMatchFailed += LobbyManager_OnQuickMatchFailed; ;

        isTimerRunning = false;
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
        queue.SetActive(false);
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
}
