using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomePlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Tokens;

    void Start()
    {
        CheckAndSetData();
    }
    private bool CheckAndSetData()
    {
        var playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return false;
        }

        Name.text = playerData.Name;
        Name.enabled = true;
        Tokens.text = playerData.Tokens.ToString();
        Tokens.enabled = true;
        return true;
    }
}
