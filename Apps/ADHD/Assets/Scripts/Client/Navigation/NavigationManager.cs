using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Transform contentView;
    [SerializeField] private GameObject startPage;

    private ButtonUI[] buttons;

    private void Awake()
    {
        buttons = GetComponentsInChildren<ButtonUI>();
    }

    void Start()
    {
        ClearContentView();
        NavigateToPage(startPage);
    }

    public void NavigateToPage(GameObject pagePrefab)
    {
        if (pagePrefab == null)
        {
            return;
        }

        ClearContentView();
        ResetButtons();
        Instantiate(pagePrefab, contentView);
    }

    private void ResetButtons()
    {
        foreach(ButtonUI button in buttons)
        {
            button.SetOff();
        }
    }

    private void ClearContentView()
    {
        foreach (Transform child in contentView)
        {
            Destroy(child.gameObject);
        }
    }
}
