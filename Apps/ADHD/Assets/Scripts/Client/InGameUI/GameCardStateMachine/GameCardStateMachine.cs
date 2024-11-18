using UnityEngine.EventSystems;

public class GameCardStateMachine
{
    private GameCardState currentState;

    public void SetState(GameCardState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void HandleBeginDrag(PointerEventData eventData) => currentState?.OnBeginDrag(eventData);
    public void HandleDrag(PointerEventData eventData) => currentState?.OnDrag(eventData);
    public void HandleEndDrag(PointerEventData eventData) => currentState?.OnEndDrag(eventData);
    public void HandlePointerEnter(PointerEventData eventData) => currentState?.OnPointerEnter(eventData);
    public void HandlePointerExit(PointerEventData eventData) => currentState?.OnPointerExit(eventData);
}

