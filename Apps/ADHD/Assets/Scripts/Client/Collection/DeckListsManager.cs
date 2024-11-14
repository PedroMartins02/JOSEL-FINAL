using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckListsManager : MonoBehaviour
{
    [SerializeField] private Transform[] DeckSlots;
    [SerializeField] private Transform[] Checks;
    [SerializeField] private GameObject PopUpArea;
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject OptionsPopUp;
    [SerializeField] private GameObject ErrorPopUp;
    [SerializeField] private GameObject ConfirmationPopUp;

    [SerializeField] private HighlightedDeckIdSO HighlightedDeckData;

    private void Start()
    {
        UpdateDeckLists();
        DisablePopUp(true);
    }

    private void UpdateDeckLists()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;

        for (int i = 0; i < DeckSlots.Count(); i++)
        {
            Transform deckSlot = DeckSlots[i];
            foreach (Transform child in deckSlot)
                Destroy(child.gameObject);

            if (i >= deckLists.Count())
                continue;

            DeckData deck = deckLists[i];

            var deckInstance = Instantiate(DeckPrefab, deckSlot);
            var deckUI = deckInstance.GetComponent<DeckUI>();
            deckUI.SetDeckData(deck);
        }
        UpdateSelectedDeckUI();
    }

    private void UpdateSelectedDeckUI()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        int selectedDeckId = playerData.SelectedDeckId;
        for (int i = 0; i < Checks.Count(); i++) {
            Checks[i].gameObject.SetActive(i == selectedDeckId);
        }
    }

    public void DisablePopUp(bool resetSelection = false)
    {
        if (resetSelection)
            HighlightedDeckData.DeckId = -1;

        PopUpArea.SetActive(false);
        foreach (Transform child in PopUpArea.transform)
        {
            if (child.name.Equals("Darken"))
                continue;
            child.gameObject.SetActive(false);
        }
    }

    private void ShowPopUp(GameObject popUp)
    {
        popUp.SetActive(true);
        PopUpArea.SetActive(true);
    }

    public void OnSlotClick(int slotIndex)
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;
        if (deckLists.Count > slotIndex)
        {
            HighlightedDeckData.DeckId = slotIndex;
            ShowPopUp(OptionsPopUp);
            Debug.Log($"Selected Deck {slotIndex}");
            return;
        }

        Debug.Log($"Create new Deck, clicked on slot {slotIndex}");
    }

    public void OnSelectClick()
    {
        DisablePopUp();

        if (HighlightedDeckData.DeckId == -1)
            return;

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData.SelectedDeckId == HighlightedDeckData.DeckId)
            return;

        playerData.SelectedDeckId = HighlightedDeckData.DeckId;
        AccountManager.Singleton.SetPlayerData(playerData, true);
        UpdateSelectedDeckUI();

        HighlightedDeckData.DeckId = -1;
    }

    public void OnEditClick()
    {
        //Go To EditDeck Scene (send selected deck data)
    }

    public void OnDeleteClick()
    {
        DisablePopUp();
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;
        if (deckLists.Count() <= 1)
        {
            ShowPopUp(ErrorPopUp);
            return;
        }
        ShowPopUp(ConfirmationPopUp);
    }

    public void ConfirmDeleteClick()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        playerData.DeckCollection.RemoveAt(HighlightedDeckData.DeckId);
        
        if (playerData.SelectedDeckId == HighlightedDeckData.DeckId)
        {
            playerData.SelectedDeckId = 0;
        } else if (playerData.SelectedDeckId > HighlightedDeckData.DeckId)
        {
            playerData.SelectedDeckId -= 1;
        }

        AccountManager.Singleton.SetPlayerData(playerData, true);
        UpdateDeckLists();
        DisablePopUp(true);
    }

    public void CancelDeleteClick()
    {
        DisablePopUp(true);
    }

    public void OnCustomizeClick()
    {
        SceneManager.LoadScene("DeckCosmeticsScene");
    }
}
