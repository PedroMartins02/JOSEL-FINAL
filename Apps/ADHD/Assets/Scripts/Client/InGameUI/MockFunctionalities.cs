using GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockFunctionalities : MonoBehaviour
{
    [SerializeField] HandCardHolder handCardHolder;

    private void Start()
    {
    }

    public void DrawCardMock(string cardSOId)
    {
        CardSO cardSO = CardDatabase.Singleton.GetCardSoOfId(cardSOId);
        handCardHolder.DrawCard(cardSO);
    }
}
