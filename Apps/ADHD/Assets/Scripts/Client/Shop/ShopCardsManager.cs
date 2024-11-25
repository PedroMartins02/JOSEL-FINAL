using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardsPageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tokensText;
    [SerializeField] private Transform contentGrid;
    [SerializeField] private GameObject cardPackPrefab;
    [SerializeField] private PackPopUp popUp;

    void Start()
    {
        UpdateContent();
    }

    private void UpdateContent()
    {
        SetTokens();
        ClearContent();
        InstantiateCardPacks();
    }

    public void AddCurrency() //for development purposes
    {
        CurrencyManager.AwardCurrency(1000);
        UpdateContent();
    }

    private void SetTokens()
    {
        tokensText.text = AccountManager.Singleton.GetPlayerData().Tokens.ToString();
    }

    private void ClearContent()
    {
        foreach (Transform t in contentGrid)
        {
            Destroy(t.gameObject);
        }
    }

    private void InstantiateCardPacks()
    {
        InstantiatePack(Factions.Greek);
        InstantiatePack(Factions.Egypt);
        InstantiatePack();
    }

    private void InstantiatePack(Factions? civilization = null)
    {
        GameObject cardPack = Instantiate(cardPackPrefab, contentGrid);
        CardPackUI cardPackUI = cardPack.GetComponent<CardPackUI>();
        cardPackUI.SetPackCivilization(civilization);
        cardPackUI.SetPurchaseOnClick(OpenPopUp);
    }

    public void OpenPopUp(CardPackUI cardPack)
    {
        popUp.SetPurchaseConfirmationHandler(ConfirmPurchaseHandler);
        popUp.ShowConfirmationPopUp(cardPack);
    }

    public void ConfirmPurchaseHandler(int price, Factions? civilization)
    {
        if (CurrencyManager.SpendCurrency(price))
        {
            List<CardSO> packContents = CardDatabase.Singleton.GetPackContents(civilization);
            if (packContents == null)
            {
                popUp.ShowErrorPopUp("Service is currently unavailable!");
                CurrencyManager.AwardCurrency(price);
            } else
            {
                popUp.ShowRewardsPopUp(packContents);
                PlayerData playerData = AccountManager.Singleton.GetPlayerData();
                foreach (CardSO card in packContents)
                {
                    playerData.CardCollection.Add(card.Id);
                }
                AccountManager.Singleton.SetPlayerData(playerData, true);
            }
        } else
        {
            popUp.ShowErrorPopUp("Not enough tokens!");
        }
        UpdateContent();
    }
}
