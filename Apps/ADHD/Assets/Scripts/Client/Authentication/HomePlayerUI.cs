using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomePlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;

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
        return true;
    }
}
