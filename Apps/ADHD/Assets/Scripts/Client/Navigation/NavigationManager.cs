using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform contentView;
    [SerializeField] private GameObject startPage;

    void Start()
    {
        ClearContentView();
        NavigateToPage(startPage);
    }

  public void NavigateToPage(GameObject pagePrefab)
  {
    ClearContentView();

    if (pagePrefab != null)
      Instantiate(pagePrefab, contentView);
  }

  private void ClearContentView()
  {
    foreach (Transform child in contentView)
    {
      Destroy(child.gameObject);
    }
  }
}
