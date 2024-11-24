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
    [SerializeField] private GameObject cardPrefab;

    [Header("Confirmation")]
    [SerializeField] private Transform purchaseConfirmationPopUp;
    [SerializeField] private Image packImage;
    [SerializeField] private TextMeshProUGUI packPrice;
    [SerializeField] private Button confirmPurchaseButton;

    [Header("Error")]
    [SerializeField] private Transform purchaseErrorPopUp;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Rewards")]
    [SerializeField] private Transform purchaseRewardsPopUp;
    [SerializeField] private Transform rewardsList;

    private CardPackUI cardPack;

    private void ShowPopUp(Transform popUp)
    {
        HidePopUps();
        popUp.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void HidePopUp()
    {
        HidePopUps();
        gameObject.SetActive(false);
    }

    private void HidePopUps()
    {
        purchaseConfirmationPopUp.gameObject.SetActive(false);
        purchaseErrorPopUp.gameObject.SetActive(false);
        purchaseRewardsPopUp.gameObject.SetActive(false);
    }

    public void ShowRewardsPopUp(List<CardSO> receivedCards)
    {
        foreach (Transform t in rewardsList)
        {
            Destroy(t.gameObject);
        }

        foreach (CardSO card in receivedCards)
        {
            GameObject cardInstance = Instantiate(cardPrefab, rewardsList);
            cardInstance.GetComponent<CardUI>().SetCardData(card);
        }

        ShowPopUp(purchaseRewardsPopUp);
    }

    public void ShowErrorPopUp(string errorText)
    {
        this.errorText.text = errorText;
        ShowPopUp(purchaseErrorPopUp);
    }

    public void ShowConfirmationPopUp(CardPackUI cardPack)
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
        ShowPopUp(purchaseConfirmationPopUp);
    }

    public void SetPurchaseConfirmationHandler(Action<int, Factions?> purchaseConfirmationHandler)
    {
        confirmPurchaseButton.onClick.RemoveAllListeners();
        confirmPurchaseButton.onClick.AddListener(() => purchaseConfirmationHandler(cardPack.price, cardPack.civilization));
    }
}
