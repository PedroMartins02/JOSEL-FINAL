using UnityEngine.EventSystems;

public class HoverState : GameCardState
{
    public HoverState(GameCard card) : base(card) { }

    public override void Enter()
    {
        card.isHovering = true;
        card.isDragging = false;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        card.PointerExitEvent.Invoke(card);
        card.stateMachine.SetState(new IdleState(card));
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!card.isMine)
            return;

        card.BeginDragEvent.Invoke(card);
        card.stateMachine.SetState(new DraggingState(card));
    }
}
