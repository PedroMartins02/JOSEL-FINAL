using ModestTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGameManager : MonoBehaviour
{
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
}
