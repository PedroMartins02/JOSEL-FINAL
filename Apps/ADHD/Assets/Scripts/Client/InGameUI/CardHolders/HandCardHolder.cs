using DG.Tweening;
using GameModel;
using System.Collections;
using System.Linq;
using UnityEngine;

public class HandCardHolder : HorizontalCardHolder
{
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
}
