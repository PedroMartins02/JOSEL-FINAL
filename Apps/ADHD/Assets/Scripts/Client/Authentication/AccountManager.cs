using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    private static AccountManager _singleton;
    public static AccountManager Singleton
    {
        get => _singleton;
        private set
        {
            if (null == _singleton)
                _singleton = value;
            else if (value != _singleton)
            {
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private PlayerData playerData;

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerData playerData, bool isToSave = false)
    {
        this.playerData = playerData;
        if (isToSave)
        {
            SaveData();
        }
    }

    public async void LoadData()
    {
        var playerDataDictionary = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {"playerData"});

        if (playerDataDictionary.TryGetValue("playerData", out var firstKey))
        {
            playerData = PlayerData.FromDictionary(firstKey.Value.GetAs<Dictionary<string, object>>());
            Debug.Log($"Loaded data {string.Join(',', playerDataDictionary)}");
            Debug.Log($"PlayerData instance {playerData.ToJson()}");
        }
    }

    private async void SaveData()
    {
        Debug.Log($"PlayerData instance {playerData.ToJson()}");
        var playerDataDictionary = new Dictionary<string, object>{
          {"playerData", playerData.ToDictionary()}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerDataDictionary);
        Debug.Log($"Saved data {string.Join(',', playerDataDictionary)}");
    }

}
