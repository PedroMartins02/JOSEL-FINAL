using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNavigationManager : MonoBehaviour
{
    [SerializeField] private Transform contentView;

    [SerializeField] private GameObject quickMatchPrefab;
    [SerializeField] private GameObject serverBrowserPrefab;
    [SerializeField] private GameObject createGamePrefab;

    void Start()
    {
        NavigateToQuickMatch();
    }

    public void NavigateToQuickMatch()
    {
        ClearContentView();
        Instantiate(quickMatchPrefab, contentView);
    }

    public void NavigateToServerBrowser()
    {
        ClearContentView();
        Instantiate(serverBrowserPrefab, contentView);
    }

    public void NavigateToCreateGame()
    {
        ClearContentView();
        Instantiate(createGamePrefab, contentView);
    }

    private void ClearContentView()
    {
        foreach (Transform child in contentView)
        {
            Destroy(child.gameObject);
        }
    }
}
