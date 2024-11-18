using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardUI : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI CardName;

    [SerializeField] private Image CardArt;

    private CardSO associatedCard;

    void Start()
    {
        
    }

    public void SetCardData(CardSO card)
    {
        if (card == null)
            return;
        
        SetGeneralUI(card);
    }

    public CardSO GetAssociatedCard()
    {
        return associatedCard;
    }

    public void Destroy()
    {
        DeckEditorManager.Instance.RemoveFromEditingArea(associatedCard);
        Destroy(gameObject);
    }
    
    private void SetGeneralUI(CardSO card)
    {
        CardName.text = card.Name;

        CardArt.sprite = card.Art;

        associatedCard = card;
    }
}
