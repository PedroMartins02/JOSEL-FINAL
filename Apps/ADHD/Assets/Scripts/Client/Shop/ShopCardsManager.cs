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
    }
}
