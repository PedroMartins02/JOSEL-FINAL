using GameModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public static class RankManager
{
    private static float recentWinRateWeight = 0.3f;
    private static float fullWinRateWeight = 0.2f;
    private static int baseMMRReward = 30;

    public static int AwardMMR(int opponentMMR, bool didWin)
    {
        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        int mmrDifference = CalculateMMRDifference(playerData.MMR, playerData.MatchHistory, opponentMMR, didWin);
        playerData.MatchHistory.Add(didWin);
        playerData.MMR += mmrDifference;
        AccountManager.Singleton.SetPlayerData(playerData, true);
        return mmrDifference;
    }

    private static int CalculateMMRDifference(int playerMMR, List<bool> matchHistory, int opponentMMR, bool didWin)
    {
        double performanceModifier = CalculatePerformanceModifier(matchHistory);
        double expectedOutcome = CalculateExpectedOutcome(playerMMR, opponentMMR);
        return (int) (baseMMRReward * ((didWin ? 1 : 0) - expectedOutcome) * (1 + performanceModifier));
    }

    private static double CalculatePerformanceModifier(List<bool> matchHistory)
    {
        if (matchHistory.Count > 10)
        {
            List<bool> recentMatchHistory = matchHistory.TakeLast(5).ToList();

            double recentWinRatio = recentMatchHistory.Count(b => b) / recentMatchHistory.Count();
            double fullWinRatio = matchHistory.Count(b => b) / matchHistory.Count();
            return recentWinRateWeight * recentWinRatio + fullWinRateWeight * (fullWinRatio - 0.5);
        }
        return recentWinRateWeight * 1 + fullWinRateWeight * (1 - 0.5);
    }

    private static double CalculateExpectedOutcome(int playerMMR, int opponentMMR)
    {
        return 1 / (1 + Math.Pow(10, (opponentMMR - playerMMR) / 400));
    }
}
