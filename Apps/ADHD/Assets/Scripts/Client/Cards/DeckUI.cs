using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private Sprite[] CardBackSprites;

    [SerializeField] private Image CardBackImage;
    [SerializeField] private MythUI MythUI;
    [SerializeField] private TextMeshProUGUI NameText;

    public void SetDeckData(DeckData data)
    {
        if (data == null)
            return;

        int cardBackId = data.CardBackId > CardBackSprites.Count() ? 0 : data.CardBackId;
        CardBackImage.sprite = CardBackSprites[cardBackId];

        MythUI.SetMythData((MythCardSO)CardDatabase.Singleton.GetCardSoOfId(data.MythId));
    
        NameText.text = data.Name;
    }
}
