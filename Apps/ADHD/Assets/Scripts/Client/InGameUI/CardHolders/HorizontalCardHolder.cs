using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Game.Data;
using Game.Logic;
using Unity.Netcode;

public abstract class HorizontalCardHolder : NetworkBehaviour
{
    [SerializeField] protected bool isMine = true;

    [SerializeField] protected GameCard selectedCard;
    [SerializeReference] protected GameCard hoveredCard;

    [SerializeField] protected GameObject slotPrefab;
    protected RectTransform rect;

    [Header("Spawn Settings")]
    [SerializeField] protected int cardsToSpawn = 7;
    public List<GameCard> cards;

    [Header("Curve")]
    [SerializeField] public CurveParameters curve;

    protected bool isCrossing = false;
    [SerializeField] protected bool tweenCardReturn = true;

    protected virtual void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    protected virtual void BeginDrag(GameCard card)
    {
        selectedCard = card;
    }

    protected virtual void EndDrag(GameCard card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;
    }

    protected void CardPointerEnter(GameCard card)
    {
        hoveredCard = card;
    }

    protected void CardPointerExit(GameCard card)
    {
        hoveredCard = null;
    }

    protected virtual void Update()
    {
        if (selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {

            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    protected void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
        cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (GameCard card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

    public void RemoveCard(GameCard card)
    {
        card.PointerEnterEvent.RemoveListener(CardPointerEnter);
        card.PointerExitEvent.RemoveListener(CardPointerExit);
        card.BeginDragEvent.RemoveListener(BeginDrag);
        card.EndDragEvent.RemoveListener(EndDrag);

        cards.Remove(card);
        UpdateIndexes();
    }

    public void AddCard(GameCard card)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);

        if (card.cardVisual != null)
        {
            card.cardVisual.curve = curve;
        }

        cards.Add(card);
        UpdateIndexes();
    }

    public void SpawnCard(CardDataSnapshot cardData, ulong playerId)
    {
        GameObject cardSlot = Instantiate(slotPrefab, transform);

        GameCard card = cardSlot.GetComponentInChildren<GameCard>();

        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);

        card.name = "unknown";
        card.isMine = NetworkManager.Singleton.LocalClientId.Equals(playerId);
        card.isInHand = true;
        card.gameObject.tag = isMine ? "MyCard" : "OpponentCard";

        card.GameID = cardData.GameID;

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);

            cards.Add(card);
            UpdateIndexes();

            if (card.isMine || cardData.CurrentState == CardStateType.InPlay)
            {
                card.name = cardData.Id;
                card.SetCardData(cardData);
            }
        }
    }

    public void UpdateIndexes()
    {
        for (int i = 0; i < cards.Count(); i++)
        {
            cards[i].cardVisual.UpdateIndex(i);
            cards[i].cardVisual.curve = curve;
        }
    }
}
