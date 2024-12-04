using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Game.Log;

public class MatchResultManager : MonoBehaviour
{
    [Header("Victory/Defeat Images")]
    [SerializeField] private Sprite VictorySprite;
    [SerializeField] private Sprite DefeatSprite;
    [SerializeField] private Image WinLossImage;
    [SerializeField] private Image ResultImage;

    [Header("Rank")]
    [SerializeField] private RankUI RankUI;

    [Header("Match Rewards")]
    [SerializeField] private TextMeshProUGUI RankDifferenceText;
    [Header("Victory")]
    [SerializeField] private GameObject VictoryReward;

    private GameLog gameLog;

    private bool didWin;

    void Start()
    {
        gameLog = GameLog.Instance; // Get all the information you need here

        SetMatchResult(gameLog.OpponentMMR, gameLog.PlayerWon);
    }

    private void SetMatchResult(int opponentMMR, bool didWin)
    {
        this.didWin = didWin;
        int mmrDifference = RankManager.AwardMMR(opponentMMR, didWin);
        UpdateUI(mmrDifference); 
        StartCoroutine(ShowResult());
    }

    private IEnumerator ShowResult()
    {
        ResultImage.sprite = didWin ? VictorySprite : DefeatSprite;
        ResultImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        ResultImage.gameObject.SetActive(false);
    }

    private void UpdateUI(int mmrDifference)
    {
        VictoryReward.SetActive(didWin);
        WinLossImage.sprite = didWin ? VictorySprite : DefeatSprite;
        CurrencyManager.AwardCurrency(didWin ? 70 : 20);
        RankDifferenceText.SetText((mmrDifference >= 0 ? "+ " : "- ") + Math.Abs(mmrDifference));
        RankUI.SetRank();
    }

    public void OnContinueClick()
    {
        Destroy(gameLog.gameObject);

        SceneLoader.Load("NavigationScene");
    }
}
