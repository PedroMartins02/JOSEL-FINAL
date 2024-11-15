using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardUI : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI CardName;

    [SerializeField] private Image CardArt;

    void Start()
    {
        
    }

    public void SetCardData(CardSO card)
    {
        if (card == null)
            return;

        SetGeneralUI(card);
    }

    private void SetGeneralUI(CardSO card)
    {
        CardName.text = card.Name;

        CardArt.sprite = card.Art;
    }
}
