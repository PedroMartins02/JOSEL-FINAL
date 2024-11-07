using ModestTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_CreateGameController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameText;

    private string lobbyName;
    private bool isPrivate = false;

    public void CreateGame()
    {
        lobbyName = nameText.text.IsEmpty() ? "Custom Game" : nameText.text;

        LobbyManager.Instance.CreateLobby(
                lobbyName,
                isPrivate,
                LobbyManager.LobbyType.CustomMatch
            );
    }


    /**
    [SerializeField] private TMP_InputField nameText;

    public async void CreateGameAsync()
    {
        var lobbyName = nameText.text.IsEmpty() ? "Custom Game" : nameText.text;
        Debug.Log("Start Loading");
        if (await RelayManager.Singleton.CreateRoom(lobbyName, "Custom"))
        {
            NavigateToLobby();
        }
        Debug.Log("Stop Loading");
    }

    private void NavigateToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    */
}
