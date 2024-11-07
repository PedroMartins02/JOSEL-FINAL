using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuickGameController : MonoBehaviour
{
    private void Start()
    {
        //LobbyManager.Instance.OnQuickMatchStarted += ;
        //LobbyManager.Instance.OnQuickMatchFailed += ;
    }


    public void FindGame()
    {
        LobbyManager.Instance.QuickMatchLobby();
    }




    /**
    private bool isInQueue = false;

    public void FindGame()
    {
        if (isInQueue) { return; }
        RelayManager.Singleton.JoinQueue();
        isInQueue = true;
    }
    */
}
