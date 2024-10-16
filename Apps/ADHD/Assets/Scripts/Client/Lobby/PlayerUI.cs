using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    public void SetPlayerData(Player player)
    {
        nameText.text = player.Profile.Name;
    }
}
