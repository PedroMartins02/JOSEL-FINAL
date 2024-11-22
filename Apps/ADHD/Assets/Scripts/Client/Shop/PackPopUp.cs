using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameModel;
using System;

public class PackPopUp : MonoBehaviour
{
    [SerializeField] private Sprite greekPack;
    [SerializeField] private Sprite egyptPack;
    [SerializeField] private Sprite randomPack;

    [SerializeField] private Image packImage;
    [SerializeField] private TextMeshProUGUI packPrice;
    [SerializeField] private Button confirmPurchaseButton;

    private CardPackUI cardPack;

    public void HidePopUp()
    {
        gameObject.SetActive(false);
    }

    public void ShowPopUp(CardPackUI cardPack)
    {
        this.cardPack = cardPack;
        switch (cardPack.civilization)
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
        packPrice.text = cardPack.price.ToString();
        gameObject.SetActive(true);
    }

    public void SetPurchaseConfirmationHandler(Action<int, Factions?> purchaseConfirmationHandler)
    {
        confirmPurchaseButton.onClick.RemoveAllListeners();
        confirmPurchaseButton.onClick.AddListener(() => purchaseConfirmationHandler(cardPack.price, cardPack.civilization));
    }
}
