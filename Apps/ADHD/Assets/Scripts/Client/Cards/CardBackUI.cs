using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardBackUI : MonoBehaviour
{
    [SerializeField] private Sprite[] CardBackSprites;

    [SerializeField] private Image CardBackImage;
    [SerializeField] private GameObject Checkmark;

    public void SetCardBack(int cardBackId)
    {
        int id = cardBackId > CardBackSprites.Count() ? 0 : cardBackId;
        CardBackImage.sprite = CardBackSprites[id];
    }

    public void UpdateCheck(bool isChecked)
    {
        Checkmark.SetActive(isChecked);
    }
}
