using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerName;

    void Start()
    {
        //SetupUI();
    }

    private async void SetupUI()
    {
        PlayerName.enabled = false;

        if (CheckAndSetPlayerData())
        {
            return;
        }

        if (await AccountManager.Singleton.LoadData())
        {
            CheckAndSetPlayerData();
        }
    }

    private bool CheckAndSetPlayerData()
    {
        var playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return false;
        }

        PlayerName.text = playerData.Name;
        PlayerName.enabled = true;
        return true;
    }
}
