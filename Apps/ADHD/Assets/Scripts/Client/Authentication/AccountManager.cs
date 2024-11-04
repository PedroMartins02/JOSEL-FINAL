using System.Collections;
using System.Collections.Generic;
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

    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
          "firstKeyName", "secondKeyName"
        });

        if (playerData.TryGetValue("firstKeyName", out var firstKey))
        {
            Debug.Log($"firstKeyName value: {firstKey.Value.GetAs<string>()}");
        }

        if (playerData.TryGetValue("secondKeyName", out var secondKey))
        {
            Debug.Log($"secondKey value: {secondKey.Value.GetAs<int>()}");
        }
    }

    public async void SaveData()
    {
        var playerData = new Dictionary<string, object>{
          {"firstKeyName", "a text value"},
          {"secondKeyName", 123}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }

}
