using UnityEngine.EventSystems;

public class UIDiscardedState : GameCardState
{
    public UIDiscardedState(GameCard card) : base(card) { }

    public override void Enter()
    {
        card.isHovering = false;
        card.isDragging = false;
    }
}
