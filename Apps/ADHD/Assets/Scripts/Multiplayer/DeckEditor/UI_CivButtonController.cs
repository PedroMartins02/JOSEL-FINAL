using GameModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CivButtonController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite greekSprite;
    [SerializeField] private Sprite romanSprite;
    [SerializeField] private Sprite norseSprite;
    [SerializeField] private Sprite egyptSprite;

    [Header("Image target")]
    [SerializeField] private Image buttonImage;

    [Header("Content UI")]
    [SerializeField] private TextMeshProUGUI civNameText;
    [SerializeField] private TextMeshProUGUI civInfoText;


    private Factions associatedFaction;

    public void SetCivData(Factions faction)
    {
        associatedFaction = faction;
        if (Factions.Greek.Equals(faction))
        {
            buttonImage.sprite = greekSprite;
        }
        else if (Factions.Roman.Equals(faction))
        {
            buttonImage.sprite = romanSprite;
        }
        else if (Factions.Norse.Equals(faction))
        {
            buttonImage.sprite = norseSprite;
        }
        else if (Factions.Egypt.Equals(faction))
        {
            buttonImage.sprite = egyptSprite;
        }
    }

    public Factions GetCivData()
    {
        return this.associatedFaction;
    }

    public void CivButtonOnClick()
    {
        civNameText.text = associatedFaction.ToString();


    }
}
