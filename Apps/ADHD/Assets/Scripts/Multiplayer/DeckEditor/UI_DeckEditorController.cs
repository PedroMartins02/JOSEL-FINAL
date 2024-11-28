using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DeckEditorController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject mythPrefab;

    [Header("Civ Information")]
    [SerializeField] private Transform civCardsContainer;
    [SerializeField] private Transform civMythsContainer;
    [SerializeField] private TextMeshProUGUI civFactsText;

    [Header("Buttons")]
    [SerializeField] private ButtonUI greekButton;
    [SerializeField] private ButtonUI egyptButton;

    private int chosenFaction = 0;

    private void Start()
    {
        ClearContainer(civCardsContainer);
        ClearContainer(civMythsContainer);
        SetCivilization(0);
    }

    public void SetCivilization(int index)
    {
        chosenFaction = index;
        if (index == 0)
        {
            greekButton.SetOn();
            egyptButton.SetOff();
            SetCards(Factions.Greek);
            SetMyths(Factions.Greek);
            SetInfo(Factions.Greek);
        } else
        {
            greekButton.SetOff();
            egyptButton.SetOn();
            SetCards(Factions.Egypt);
            SetMyths(Factions.Egypt);
            SetInfo(Factions.Egypt);
        }
    }

    private void SetCards(Factions civilization)
    {
        ClearContainer(civCardsContainer);
        for (int i = 0; i < 3; i++)
        {
            CardSO card = CardDatabase.Singleton.GetRandomCard(civilization);
            var cardInstance = Instantiate(cardPrefab, civCardsContainer);
            var cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCardData(card);
        }
    }

    private void SetMyths(Factions civilization)
    {
        ClearContainer(civMythsContainer);
        var allMyths = CardDatabase.Singleton.GetAllMyths(civilization);
        foreach (MythCardSO myth in allMyths)
        {
            var mythInsance = Instantiate(mythPrefab, civMythsContainer);
            var mythUI = mythInsance.GetComponent<MythUI>();
            mythUI.SetMythData(myth);
        }
    }

    private void SetInfo(Factions civilization)
    {
        civFactsText.text = civilization == Factions.Greek ? "GREEK INFO" : "EGYPT INFO";
    }

    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    public void ReturnButtonOnClick()
    {
        SceneLoader.Load(SceneLoader.Scene.NavigationScene);
    }

    public void ContinueOnClick()
    {
        PlayerPrefs.SetInt("ChosenFaction", chosenFaction);
        SceneLoader.Load(SceneLoader.Scene.DeckBuilder);
    }
}
