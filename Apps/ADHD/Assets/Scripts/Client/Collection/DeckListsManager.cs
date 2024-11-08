using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckListsManager : MonoBehaviour
{
    [SerializeField] private Transform[] DeckSlots;
    [SerializeField] private GameObject DeckPrefab;

    private void Start()
    {
        UpdateDeckLists();
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
            //var deckUI = cardInstance.GetComponent<DeckUI>();
            //deckUI.SetDeckData(deck);
        }
    }

    public void OnSlotClick(int slotIndex)
    {

    }
}
