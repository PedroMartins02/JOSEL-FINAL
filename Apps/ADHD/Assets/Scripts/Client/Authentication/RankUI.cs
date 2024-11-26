using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUI : MonoBehaviour
{
    [SerializeField] private Image rankIcon;
    [SerializeField] private Sprite[] RankSprites;

    void Start()
    {
        SetRank();
    }

    public void RandomizeRank()
    {
        var playerData = AccountManager.Singleton.GetPlayerData();
        playerData.MMR = Random.Range(400, 1001);
        AccountManager.Singleton.SetPlayerData(playerData, true);
        SetRank();
    }

    private bool SetRank()
    {
        var playerData = AccountManager.Singleton.GetPlayerData();
        if (playerData == null)
        {
            return false;
        }

        rankIcon.sprite = CalculateRankSprite(playerData.MMR);
        rankIcon.enabled = true;
        return true;
    }

    private Sprite CalculateRankSprite(int mmr)
    {
        int baseMMR = 400;
        int rankCount = RankSprites.Length;
        int maxMMR = 1000;

        float mmrRange = maxMMR - baseMMR;
        float gapPerRank = mmrRange / (rankCount - 1);

        int rankIndex = Mathf.Clamp(Mathf.FloorToInt((mmr - baseMMR) / gapPerRank), 0, rankCount - 1);

        return RankSprites[rankIndex];
    }
}
