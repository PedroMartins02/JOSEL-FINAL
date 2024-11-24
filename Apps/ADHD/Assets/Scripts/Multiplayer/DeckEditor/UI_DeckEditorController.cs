using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class UI_DeckEditorController : MonoBehaviour
{
    [Header("Civilization Buttons")]
    [SerializeField] private Transform civButtonContainer;
    [SerializeField] private Transform civButtonTemplate;


    private void Awake()
    {
        // Hide the deck Template
        civButtonTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        ClearCivButtonList();
        InstantiateCivButtons();
    }

    private void ClearCivButtonList()
    {
        if (civButtonContainer != null)
            foreach (Transform child in civButtonContainer)
            {
                if (child == civButtonTemplate) continue;
                Destroy(child.gameObject);
            }
    }

    private void InstantiateCivButtons()
    {
        int childCount = 0;
        foreach (Factions faction in Enum.GetValues(typeof(Factions)))
        {
            Transform civTransform = Instantiate(civButtonTemplate, civButtonContainer);
            civTransform.gameObject.SetActive(true);
            childCount++;
        }

        RectTransform contentRect = civButtonContainer.GetComponent<RectTransform>();
        float totalWidth = 250 * childCount + 25 * (childCount - 1);
        contentRect.sizeDelta = new Vector2(totalWidth, contentRect.sizeDelta.y);
    }

    public void ReturnButtonOnClick()
    {
        SceneLoader.Load(SceneLoader.Scene.NavigationScene);
    }

    public void ContinueOnClick()
    {
        // Verify if there is a chosen civilization

        SceneLoader.Load(SceneLoader.Scene.DeckBuilder);
    }
}
