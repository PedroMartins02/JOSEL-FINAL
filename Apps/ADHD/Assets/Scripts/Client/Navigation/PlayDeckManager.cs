using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDeckManager : MonoBehaviour
{
    [SerializeField] private Transform DeckSlot;
    [SerializeField] private GameObject DeckPrefab;

    private void Start()
    {
        UpdateSelectedDeck();
    }

    private void UpdateSelectedDeck()
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        List<DeckData> deckLists = playerData.DeckCollection;

        int deckId = playerData.SelectedDeckId > deckLists.Count ? 0 : playerData.SelectedDeckId;

        DeckData deck = deckLists[deckId];

        foreach (Transform child in DeckSlot)
        {
            Destroy(child.gameObject);
        }

        var deckInstance = Instantiate(DeckPrefab, DeckSlot);
        var deckUI = deckInstance.GetComponent<DeckUI>();
        deckUI.SetDeckData(deck);
    }
}
