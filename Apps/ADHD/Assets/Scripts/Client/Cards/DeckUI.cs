using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private CardBackUI cardBackUI;
    [SerializeField] private MythUI MythUI;
    [SerializeField] private TextMeshProUGUI NameText;

    public void SetDeckData(DeckData data)
    {
        if (data == null)
            return;

        cardBackUI.SetCardBack(data.CardBackId);

        MythUI.SetMythData((MythCardSO)CardDatabase.Singleton.GetCardSoOfId(data.MythId));
    
        NameText.text = data.Name;
    }

    public void UpdateCardBack(int cardBackId)
    {
        cardBackUI.SetCardBack(cardBackId);
    }
}
