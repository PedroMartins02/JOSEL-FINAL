using UnityEngine.EventSystems;

public class IdleState : GameCardState
{
    public IdleState(GameCard card) : base(card) { }

    public override void Enter()
    {
        card.isHovering = false;
        card.isDragging = false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        card.PointerEnterEvent.Invoke(card);
        card.stateMachine.SetState(new HoverState(card));
    }

    public override void Update()
    {
        card.ClampPosition();
    }
}
