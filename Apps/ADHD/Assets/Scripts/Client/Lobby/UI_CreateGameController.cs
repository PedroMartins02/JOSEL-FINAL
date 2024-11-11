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

    private List<GameModel.GameRule> gameRules = new List<GameModel.GameRule>();
    private string lobbyName;
    private bool isPrivate = false;

    public void CreateGame()
    {
        lobbyName = nameText.text.IsEmpty() ? "Custom Game" : nameText.text;

        LobbyManager.Instance.CreateLobby(
                lobbyName,
                isPrivate,
                LobbyManager.LobbyType.CustomMatch,
                gameRules
            );
    }
}
