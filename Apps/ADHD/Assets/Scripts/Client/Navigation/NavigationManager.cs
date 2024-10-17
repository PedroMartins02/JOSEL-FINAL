using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
  [SerializeField] private Transform contentView;
  //[SerializeField] private GameObject parallaxObject;

  void Start()
  {
    ClearContentView();
    //ShowBackground();
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

  /*private void ShowBackground()
  {
    parallaxObject.SetActive(true);
  }*/
}
