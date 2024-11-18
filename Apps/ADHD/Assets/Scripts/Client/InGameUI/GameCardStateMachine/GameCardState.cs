using UnityEngine.EventSystems;

public abstract class GameCardState
{
    protected GameCard card;

    public GameCardState(GameCard card)
    {
        this.card = card;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void OnBeginDrag(PointerEventData eventData) { }
    public virtual void OnDrag(PointerEventData eventData) { }
    public virtual void OnEndDrag(PointerEventData eventData) { }
    public virtual void OnPointerEnter(PointerEventData eventData) { }
    public virtual void OnPointerExit(PointerEventData eventData) { }
}


