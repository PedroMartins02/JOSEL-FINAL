using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockFunctionalities : MonoBehaviour
{
    [SerializeField] HorizontalCardHolder cardHolder;
    [SerializeField] PlayerInfo playerInfo;

    private void Start()
    {
    }

    public void DrawCardMock(string cardSOId)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(cardSOId);
        cardHolder.SpawnCard(cardSO);
    }

    public void DrawCardMock()
    {
        CardSO cardSO = CardDatabase.Singleton.GetRandomCard();
        cardHolder.SpawnCard(cardSO);
    }

    public void SetPlayerMyth(string mythId)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(mythId);
        playerInfo.SetMythImage(cardSO.Art);
    }

    public void SetPlayerHealth(int n)
    {
        playerInfo.SetHealth(n);
    }

    public void SetPlayerBlessings(int n)
    {
        playerInfo.SetBlessings(n);
    }
}
