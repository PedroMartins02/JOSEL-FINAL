using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPackUI : MonoBehaviour
{
    [SerializeField] private Sprite greekPack;
    [SerializeField] private Sprite egyptPack;
    [SerializeField] private Sprite randomPack;

    [SerializeField] private TextMeshProUGUI packNameText;
    [SerializeField] private Image packImage;
    [SerializeField] private TextMeshProUGUI packPriceText;

    private int price;
    private Factions? civilization;

    public void SetPackCivilization(Factions? civilization)
    {
        this.civilization = civilization;
        SetPackName();
        SetPackImage();
        SetPackPrice();
    }

    private void SetPackName()
    {
        packNameText.text = civilization == null ? "Random Pack" : civilization.ToString() + " Pack";
    }

    private void SetPackImage()
    {
        switch (civilization)
        {
            case Factions.Greek:
                packImage.sprite = greekPack;
                break;
            case Factions.Egypt:
                packImage.sprite = egyptPack;
                break;
            default:
                packImage.sprite = randomPack;
                break;
        }
    }

    private void SetPackPrice()
    {
        packPriceText.text = (civilization == null ? 100 : 200).ToString();
    }
}
