using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform contentView;

    [SerializeField] private GameObject homePrefab;
    [SerializeField] private GameObject playPrefab;
    [SerializeField] private GameObject collectionPrefab;
    [SerializeField] private GameObject settingsPrefab;

    void Start()
    {
        NavigateToHome();
    }

    public void NavigateToHome()
    {
        ClearContentView();

    }

    public void NavigateToPlay()
    {
        ClearContentView();
        GameObject playInstance = Instantiate(playPrefab, contentView);
    }

    public void NavigateToCollection()
    {
        ClearContentView();

    }

    public void NavigateToSettings()
    {
        ClearContentView();

    }

    private void ClearContentView()
    {
        foreach (Transform child in contentView)
        {
            Destroy(child.gameObject);
        }
    }
}
