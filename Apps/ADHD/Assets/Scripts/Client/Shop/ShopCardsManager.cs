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
        popUp.ShowPopUp(cardPack);
    }

    public void ConfirmPurchaseHandler(int price, Factions? civilization)
    {
        Debug.Log($"Purchasing a {(civilization == null ? "Random" : civilization.ToString())} pack for {price} tokens");
        popUp.HidePopUp();
    }
}
