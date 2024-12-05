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

    private string greekInfo = "The Greek civilization stands as a beacon of philosophy, warfare, and artistry. Rooted in city-states like Athens and Sparta, ancient Greece was a land of contrasts—where democracy thrived in one polis and martial discipline defined another. The Greeks were masters of strategy, evidenced by their phalanx formations and naval prowess, as well as pioneers of cultural achievements in theater, architecture, and the sciences. Guided by their pantheon of gods, they sought arete, or excellence, in every endeavor. In battle, Greek forces blend the precision of the disciplined hoplite with the agility of light infantry and the cunning of naval commanders.";
    private string egyptInfo = "The Egyptian civilization is one of the world's oldest and most enduring, thriving along the fertile banks of the Nile River. Known for their monumental architecture, such as the Great Pyramids and temples dedicated to their gods, the Egyptians were deeply spiritual, guided by beliefs in the afterlife and divine kingship. Their armies, led by pharaohs, combined swift chariotry with disciplined infantry, reflecting their resourceful adaptation to the harsh desert and fertile floodplains. Masters of engineering and agriculture, the Egyptians balanced innovation and mysticism to maintain their vast empire.";

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
        civFactsText.text = civilization == Factions.Greek ? greekInfo : egyptInfo;
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
        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.DeckBuilder);
    }
}
