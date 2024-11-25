using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private GameObject LoadingPanel;

    void Start()
    {
        SetLoading(true);
        SetupUI();
    }

    public void SetLoading(bool loadingState)
    {
        LoadingPanel.SetActive(loadingState);
        //...
    }

    private void SetupUI()
    {
        PlayerName.enabled = false;

        CheckAndSetPlayerData();
    }

    private bool CheckAndSetPlayerData()
    {
        var playerData = AccountManager.Singleton.GetPlayerData();
        SetLoading(false);
        if (playerData == null)
        {
            return false;
        }

        PlayerName.text = playerData.Name;
        PlayerName.enabled = true;
        return true;
    }
}
