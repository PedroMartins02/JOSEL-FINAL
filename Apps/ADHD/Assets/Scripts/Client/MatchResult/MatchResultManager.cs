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
