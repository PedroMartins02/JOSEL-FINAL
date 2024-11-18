using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using GameModel;
using System;

public class GameCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform cardTransform;
    [NonSerialized] public Canvas canvas;
    private GraphicRaycaster raycaster;
    [NonSerialized] public Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    private VisualCardsHandler visualHandler;
    [NonSerialized] public Vector3 offset;

    [Header("Movement")]
    [SerializeField] public float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    [NonSerialized] public float selectionOffset = 50;
    [NonSerialized] public float hoveringOffset = 1;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public GameCardVisual cardVisual;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    [Header("Events")]
    [HideInInspector] public UnityEvent<GameCard> PointerEnterEvent;
    [HideInInspector] public UnityEvent<GameCard> PointerExitEvent;
    [HideInInspector] public UnityEvent<GameCard> BeginDragEvent;
    [HideInInspector] public UnityEvent<GameCard> EndDragEvent;

    [Header("Positions")]
    [SerializeField] public bool isInHand = true;
    [SerializeField] public bool isMine = true;

    private BoardCardHolder detectedBoardHolder;
    private GameCard detectedCard;
    private GameObject detectedOpponent;

    public GameCardStateMachine stateMachine;

    [HideInInspector] public Card cardData;

    void Start()
    {
        cardTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();

        if (!instantiateVisual)
            return;

        visualHandler = FindObjectOfType<VisualCardsHandler>();
        cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<GameCardVisual>();
        cardVisual.Initialize(this);

        stateMachine = new GameCardStateMachine();
        stateMachine.SetState(new IdleState(this));
    }

    void Update() => stateMachine.Update();
    public void OnBeginDrag(PointerEventData eventData) => stateMachine.HandleBeginDrag(eventData);
    public void OnDrag(PointerEventData eventData) => stateMachine.HandleDrag(eventData);
    public void OnEndDrag(PointerEventData eventData) => stateMachine.HandleEndDrag(eventData);
    public void OnPointerEnter(PointerEventData eventData) => stateMachine.HandlePointerEnter(eventData);
    public void OnPointerExit(PointerEventData eventData) => stateMachine.HandlePointerExit(eventData);

    public void SetCardData(CardSO cardSO)
    {
        cardData = CreateCardInstance(cardSO);
        cardVisual.SetCardData(cardSO);
    }

    public void UpdateCardData(Card card)
    {
        cardData = card;
        cardVisual.UpdateCardData(card);
    }

    //this should be moved somewhere else, accessible for other classes, since it´s useful in multiple cases
    private Card CreateCardInstance(CardSO cardSO)
    {
        Type cardSOType = cardSO.GetType();
        if (cardSOType == typeof(UnitCardSO))
        {
            return new UnitCard((UnitCardSO)cardSO);
        }
        else if (cardSOType == typeof(BattleTacticCardSO))
        {
            return new BattleTacticCard((BattleTacticCardSO)cardSO);
        } 
        else if (cardSOType == typeof (LegendCardSO))
        {
            return new LegendCard((LegendCardSO)cardSO);
        }
        return null;
    }

    public void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public List<RaycastResult> PerformRaycast(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(eventData, raycastResults);
        return raycastResults;
    }

    private void HighlightDetectedBoardHolder(GameObject boardHolderObject)
    {
        BoardCardHolder boardHolder = boardHolderObject.GetComponent<BoardCardHolder>();
        if (detectedBoardHolder != boardHolder)
        {
            ResetDetectedBoardHolderHighlight();
            detectedBoardHolder = boardHolder;
        }
        detectedBoardHolder.ChangeHighlightState(true);
    }

    private void ResetDetectedBoardHolderHighlight()
    {
        if (detectedBoardHolder != null)
        {
            detectedBoardHolder.ChangeHighlightState(false);
            detectedBoardHolder = null;
        }
    }

    public void HandlePlayerBoardHighlight(List<RaycastResult> raycastResults)
    {
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("PlayerBoardArea"))
            {
                HighlightDetectedBoardHolder(result.gameObject);
                return;
            }
        }
        ResetDetectedBoardHolderHighlight();
    }

    private void HighlightDetectedCard(GameCard targetCard)
    {
        if (detectedCard != targetCard)
        {
            ResetDetectedCardHighlight();
            detectedCard = targetCard;
        }
        detectedCard.ChangeHighlightState(true);
        ResetDetectedOpponentHighlight();
    }

    private void HighlightDetectedOpponent(GameObject opponentObject)
    {
        if (detectedOpponent != opponentObject)
        {
            ResetDetectedOpponentHighlight();
            detectedOpponent = opponentObject;
        }
        ChangeOpponentHighlightState(detectedOpponent, true);
        ResetDetectedCardHighlight();
    }

    private void ResetDetectedCardHighlight()
    {
        if (detectedCard != null)
        {
            detectedCard.ChangeHighlightState(false);
            detectedCard = null;
        }
    }

    private void ResetDetectedOpponentHighlight()
    {
        if (detectedOpponent != null)
        {
            ChangeOpponentHighlightState(detectedOpponent, false);
            detectedOpponent = null;
        }
    }

    private void ResetOpponentInteractionHighlights()
    {
        ResetDetectedCardHighlight();
        ResetDetectedOpponentHighlight();
    }

    public void HandleOpponentInteractionHighlight(List<RaycastResult> raycastResults)
    {
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("OpponentCard"))
            {
                HighlightDetectedCard(result.gameObject.GetComponent<GameCard>());
                return;
            }

            if (result.gameObject.CompareTag("OpponentTarget"))
            {
                HighlightDetectedOpponent(result.gameObject);
                return;
            }
        }
        ResetOpponentInteractionHighlights();
    }

    public void HandleCardDropOnPlayerBoard(List<RaycastResult> raycastResults)
    {
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("PlayerBoardArea"))
            {
                if (detectedBoardHolder != null)
                {
                    detectedBoardHolder.ChangeHighlightState(false);
                    detectedBoardHolder = null;
                }
                PlayCard(result.gameObject);
                Debug.Log("Move Card to Game Board");
            }
        }
    }

    public void HandleCardAttackOrOpponentTarget(List<RaycastResult> raycastResults)
    {
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("OpponentTarget"))
            {
                AttackOpponent(result.gameObject);
                Debug.Log("Attack Opponent");
                break;
            }
            else if(result.gameObject.CompareTag("OpponentCard"))
            {
                AttackCard(result.gameObject);
                Debug.Log("Attack Enemy Card");
                break;
            }
             
        }

        ResetOpponentInteractionHighlights();
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        if (cardVisual != null)
            Destroy(cardVisual.gameObject);
    }

    public void ChangeHighlightState(bool isHighlighted, float scaleAmount = 1.25f)
    {
        cardVisual.highlight.color = isHighlighted ? new Color(1f, 0.3f, 0.3f, 0.3f) : new Color(1f, 1f, 1f, 0f);
        cardVisual.gameObject.transform.localScale = isHighlighted ? new Vector3(scaleAmount, scaleAmount, 1) : new Vector3(1, 1, 1);
    }

    private void ChangeOpponentHighlightState(GameObject opponent, bool isHighlighted)
    {
        opponent.GetComponent<Image>().color = isHighlighted ? new Color(1f, 0.3f, 0.3f, 0.3f) : new Color(1f, 1f, 1f, 0f);
    }

    public IEnumerator ResetDragAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        wasDragged = false;
    }

    private void PlayCard(GameObject playerBoardArea)
    {
        // TRY PLAY CARD 
        // if can't play card, don't run the code below

        HorizontalCardHolder horizontalCardHolder = GetComponentInParent<HorizontalCardHolder>();
        horizontalCardHolder.RemoveCard(this);

        RectTransform cardSlotTransform = cardTransform.parent as RectTransform;
        cardSlotTransform.SetParent(playerBoardArea.transform);

        horizontalCardHolder = GetComponentInParent<HorizontalCardHolder>();
        horizontalCardHolder.AddCard(this);

        isInHand = false;
    }

    private void AttackCard(GameObject targetCard)
    {
        // TRY ATTACK CARD
    }

    private void AttackOpponent(GameObject targetOpponent)
    {
        // TRY ATTACK OPPONENT
    }
}
