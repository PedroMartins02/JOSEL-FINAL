using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggingState : GameCardState
{
    public DraggingState(GameCard card) : base(card) { }

    public override void Enter()
    {
        card.isHovering = false;
        card.isDragging = true;

        card.canvas.GetComponent<GraphicRaycaster>().enabled = false;
        card.imageComponent.raycastTarget = false;

        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane));
        card.offset = mousePosition - (Vector2)card.transform.position;
    }

    public override void Update()
    {
        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane)) - card.offset;
        Vector2 direction = (targetPosition - (Vector2)card.transform.position).normalized;
        Vector2 velocity = direction * Mathf.Min(card.moveSpeedLimit, Vector2.Distance(card.transform.position, targetPosition) / Time.deltaTime);
        card.transform.Translate(velocity * Time.deltaTime);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = card.PerformRaycast(eventData);

        if (card.isInHand)
        {
            card.HandlePlayerBoardHighlight(raycastResults);
        }
        else
        {
            card.HandleOpponentInteractionHighlight(raycastResults);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        card.EndDragEvent.Invoke(card);
        
        card.canvas.GetComponent<GraphicRaycaster>().enabled = true;
        card.imageComponent.raycastTarget = true;

        List<RaycastResult> raycastResults = card.PerformRaycast(eventData);

        if (card.isInHand)
        {
            card.HandleCardDropOnPlayerBoard(raycastResults);
        }
        else
        {
            card.HandleCardAttackOrOpponentTarget(raycastResults);
        }

        card.StartCoroutine(card.ResetDragAfterFrame());
        card.stateMachine.SetState(new IdleState(card));
    }
}
