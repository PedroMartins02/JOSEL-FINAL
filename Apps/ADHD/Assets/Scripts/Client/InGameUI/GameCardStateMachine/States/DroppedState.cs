using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppedState : GameCardState
{
    public DroppedState(GameCard card) : base(card) { }

    public override void Enter()
    {
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
    }
}

