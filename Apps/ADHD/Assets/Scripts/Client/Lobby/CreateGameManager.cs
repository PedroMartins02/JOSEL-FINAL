using ModestTree;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateGameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameText;

    public void CreateGame()
    {
        var lobbyName = nameText.text.IsEmpty() ? "Custom Game" : nameText.text;
        RelayManager.Singleton.CreateRoom(lobbyName, "Custom");
    }
}
