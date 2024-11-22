using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurrencyManager
{
    public static void AwardCurrency(int value)
    {
        if (value <= 0) return;

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        playerData.Tokens += value;
        AccountManager.Singleton.SetPlayerData(playerData, true);
    }

    public static bool SpendCurrency(int value)
    {
        if (value < 0) return false;

        PlayerData playerData = AccountManager.Singleton.GetPlayerData();
        int playerTokens = playerData.Tokens;

        if (value > playerTokens) return false;

        playerTokens -= value;
        playerData.Tokens = playerTokens;
        AccountManager.Singleton.SetPlayerData(playerData, true);

        return true;
    }
}
