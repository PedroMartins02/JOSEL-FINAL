using DG.Tweening;
using GameModel;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HandCardHolder : HorizontalCardHolder
{

    protected override void Start()
    {
        for (int i = 0; i < cardsToSpawn; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        cards = GetComponentsInChildren<GameCard>().ToList();
        int cardCount = 0;

        foreach (GameCard card in cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            card.isMine = isMine;
            card.isInHand = true;
            card.gameObject.tag = isMine ? "MyCard" : "OpponentCard";
            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].cardVisual != null)
                    cards[i].cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

    protected override void EndDrag(GameCard card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;

        //Updated Visual Indexes
        foreach (GameCard c in cards)
        {
            c.cardVisual.UpdateIndex(transform.childCount);
        }
    }

    public void DrawCard(CardSO cardSO)
    {
        GameObject cardSlot = Instantiate(slotPrefab, transform);

        GameCard card = cardSlot.GetComponentInChildren<GameCard>();

        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);

        card.name = cardSO.Id;
        card.isMine = isMine;
        card.isInHand = true;
        card.gameObject.tag = isMine ? "MyCard" : "OpponentCard";

        card.SetCardData(cardSO);

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            if (card.cardVisual != null)
                card.cardVisual.UpdateIndex(transform.childCount);

            cards.Add(card);
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }
}
