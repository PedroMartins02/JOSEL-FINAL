using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckListsManager : MonoBehaviour
{
    [SerializeField] private Transform[] DeckSlots;
    [SerializeField] private Transform[] Checks;
    [SerializeField] private GameObject DeckPrefab;
    [SerializeField] private GameObject PopUp;

    private int highlightedDeckId = -1;

    private void Start()
    {
        UpdateDeckLists();
        DisablePopUp();
    }

    private void UpdateDeckLists()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;

        int deckCount = deckLists.Count > 6 ? 6 : deckLists.Count;

        for (int i = 0; i < deckCount; i++)
        {
            DeckData deck = deckLists[i];
            Transform deckSlot = DeckSlots[i];

            foreach (Transform child in deckSlot)
            {
                Destroy(child.gameObject);
            }

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

    public void DisablePopUp()
    {
        highlightedDeckId = -1;
        PopUp.SetActive(false);
    }

    public void OnSlotClick(int slotIndex)
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;
        if (deckLists.Count > slotIndex)
        {
            highlightedDeckId = slotIndex;
            PopUp.SetActive(true);
            Debug.Log($"Selected Deck {slotIndex}");
            return;
        }

        Debug.Log($"Create new Deck, clicked on slot {slotIndex}");
    }

    public void OnSelectClick()
    {
        DisablePopUp();
        if (highlightedDeckId == -1)
            return;

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData.SelectedDeckId == highlightedDeckId)
            return;

        playerData.SelectedDeckId = highlightedDeckId;
        AccountManager.Singleton.SetPlayerData(playerData, true);
        UpdateSelectedDeckUI();
    }

    public void OnEditClick()
    {
        //Go To EditDeck Scene (send selected deck data)
    }

    public void OnDeleteClick()
    {
        //
    }

    public void OnCustomizeClick()
    {
        //
    }
}
