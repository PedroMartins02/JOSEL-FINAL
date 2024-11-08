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
    [SerializeField] private Image MythImage;
    [SerializeField] private TextMeshProUGUI NameText;

    public void SetDeckData(DeckData data)
    {
        if (data == null)
            return;

        int cardBackId = data.CardBackId > CardBackSprites.Count() ? 0 : data.CardBackId;
        CardBackImage.sprite = CardBackSprites[cardBackId];
    
        NameText.text = data.Name;
    }
}
