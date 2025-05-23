using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InstanceCleanup : MonoBehaviour
{
    private void Awake()
    {
        if (MultiplayerManager.Instance != null)
        {
            Destroy(MultiplayerManager.Instance.gameObject);
        }
        if (LobbyManager.Instance != null)
        {
            Destroy(LobbyManager.Instance.gameObject);
        }
    }
}
