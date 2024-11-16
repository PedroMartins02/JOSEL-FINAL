using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GameModel;

public class GameCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform cardTransform;
    private Canvas canvas;
    private GraphicRaycaster raycaster;
    private Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    public float hoveringOffset = 1;

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

    [SerializeField] public bool isInHand = true;
    [SerializeField] public bool isMine = true;
    private BoardCardHolder detectedBoardHolder;
    private GameCard detectedCard;
    private GameObject detectedOpponent;

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
    }

    public void SetCardData(CardSO cardSO)
    {
        cardVisual.SetCardData(cardSO);
    }

    public void UpdateCardData(Card card)
    {
        cardVisual.UpdateCardData(card);
    }

    void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 pointerPosition = Pointer.current.position.ReadValue();
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane)) - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        isDragging = true;
        BeginDragEvent.Invoke(this);

        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane));
        offset = mousePosition - (Vector2)transform.position;
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;

        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(eventData, raycastResults);

        isInHand = GetComponentInParent<HandCardHolder>() != null;
        if (isInHand)
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.CompareTag("PlayerBoardArea"))
                {
                    if (detectedBoardHolder == null)
                    {
                        detectedBoardHolder = result.gameObject.GetComponent<BoardCardHolder>();
                    }
                    detectedBoardHolder.ChangeHighlightState(true);
                    return;
                }
            }
            if (detectedBoardHolder != null)
            {
                detectedBoardHolder.ChangeHighlightState(false);
            }
            return;
        }
        else
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.CompareTag("OpponentCard") && !result.gameObject.GetComponent<GameCard>().isInHand)
                {
                    if (detectedCard != result.gameObject.GetComponent<GameCard>())
                    {
                        if (detectedCard != null)
                        {
                            detectedCard.ChangeHighlightState(false);
                        }
                        detectedCard = result.gameObject.GetComponent<GameCard>();
                    }
                    detectedCard.ChangeHighlightState(true);
                    if (detectedOpponent != null)
                    {
                        ChangeOpponentHighlightState(detectedOpponent, false);
                    }
                    return;
                }
                else if (result.gameObject.CompareTag("OpponentTarget"))
                {
                    if (detectedOpponent != result.gameObject)
                    {
                        if (detectedOpponent != null)
                        {
                            ChangeOpponentHighlightState(detectedOpponent, true);
                        }
                        detectedOpponent = result.gameObject;
                    }
                    ChangeOpponentHighlightState(detectedOpponent, true);
                    if (detectedCard != null)
                    {
                        detectedCard.ChangeHighlightState(false);
                    }
                    return;
                }

                if (detectedCard != null)
                {
                    detectedCard.ChangeHighlightState(false);
                }
                if (detectedOpponent != null)
                {
                    ChangeOpponentHighlightState(detectedOpponent, false);
                }
                return;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        isDragging = false;
        EndDragEvent.Invoke(this);
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(eventData, raycastResults);

        isInHand = GetComponentInParent<HandCardHolder>() != null;
        if (isInHand)
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
        else
        {
            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.CompareTag("OpponentCard") && !result.gameObject.GetComponent<GameCard>().isInHand)
                {
                    AttackCard(result.gameObject);
                    Debug.Log("Attack Enemy Card");
                }
                else if (result.gameObject.CompareTag("OpponentTarget"))
                {
                    AttackOpponent(result.gameObject);
                    Debug.Log("Attack Opponent");
                }
            }
            if (detectedCard != null)
            {
                detectedCard.ChangeHighlightState(false);
                detectedCard = null;
            }
            if (detectedOpponent != null)
            {
                ChangeOpponentHighlightState(detectedOpponent, false);
                detectedOpponent = null;
            }
        }

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        PointerEnterEvent.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        PointerExitEvent.Invoke(this);
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
        cardVisual.sprite.color = isHighlighted ? new Color(1f, 0.3f, 0.3f, 1f) : Color.white;
        cardVisual.gameObject.transform.localScale = isHighlighted ? new Vector3(scaleAmount, scaleAmount, 1) : new Vector3(1, 1, 1);
    }

    private void ChangeOpponentHighlightState(GameObject opponent, bool isHighlighted)
    {
        opponent.GetComponent<Image>().color = isHighlighted ? new Color(1f, 0.3f, 0.3f, 0.3f) : new Color(1f, 1f, 1f, 0f);
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
