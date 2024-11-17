using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockFunctionalities : MonoBehaviour
{
    [SerializeField] HorizontalCardHolder cardHolder;

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
}
