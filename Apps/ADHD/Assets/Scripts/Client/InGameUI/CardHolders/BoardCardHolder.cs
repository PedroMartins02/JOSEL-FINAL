using System.Collections;
using System.Linq;
using UnityEngine;

public class BoardCardHolder : HorizontalCardHolder
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
            card.isInHand = false;
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

    protected override void Update()
    {
        //don't do anything
    }

    public void ChangeHighlightState(bool isHighlighted)
    {
        gameObject.transform.localScale = isHighlighted ? new Vector3(1.2f, 1, 1) : new Vector3(1, 1, 1);
    }
}
