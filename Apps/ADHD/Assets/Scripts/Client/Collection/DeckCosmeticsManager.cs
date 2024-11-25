using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckCosmeticsManager : MonoBehaviour
{
    [SerializeField] private Transform ScrollContent;
    [SerializeField] private GameObject CardBackPrefab;

    [SerializeField] private DeckUI SelectedDeckUI;

    [SerializeField] private HighlightedDeckIdSO HighlightedDeckData;

    private DeckData currentDeck;

    private void Start()
    {
        UpdateDeckUI();
        UpdateCardBacksList();
    }

    private void UpdateDeckUI()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;

        if (deckLists.Count < HighlightedDeckData.DeckId)
            return;

        currentDeck = deckLists[HighlightedDeckData.DeckId];

        SelectedDeckUI.SetDeckData(currentDeck);
    }

    private void UpdateCardBacksList()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();

        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }

        foreach (int cardBackId in playerData.CardBackCollection)
        {
            var cardBackInstance = Instantiate(CardBackPrefab, ScrollContent);
            var cardBackUI = cardBackInstance.GetComponent<CardBackUI>();
            cardBackUI.SetCardBack(cardBackId);

            Button cardBackButton = cardBackInstance.GetComponent<Button>();

            if (cardBackButton == null)
                return;

            cardBackButton.onClick.RemoveAllListeners();
            cardBackButton.onClick.AddListener(() => SelectCardBack(cardBackId));

            cardBackUI.UpdateCheck(cardBackId == currentDeck.CardBackId);
        }
    }

    private void SelectCardBack(int cardBackId)
    {
        currentDeck.CardBackId = cardBackId;
        SelectedDeckUI.SetDeckData(currentDeck);
        UpdateCardBacksList();
    }

    public void OnBackClick()
    {
        SaveSelection();
        SceneLoader.ExitNetworkLoad(SceneLoader.Scene.NavigationScene);
    }

    private void SaveSelection()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        playerData.DeckCollection[HighlightedDeckData.DeckId] = currentDeck;
        AccountManager.Singleton.SetPlayerData(playerData, true);
    }
}
