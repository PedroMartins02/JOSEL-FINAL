using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GameModel;
using TMPro;

public class GameCardVisual : MonoBehaviour
{
    private bool initalize = false;

    [Header("Card")]
    public GameCard parentCard;
    private Transform cardTransform;
    private Vector3 rotationDelta;
    private int savedIndex;
    Vector3 movementDelta;
    private Canvas canvas;

    [Header("References")]
    public Transform visualShadow;
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform tiltParent;
    [SerializeField] private Image shadowImage;
    [SerializeField] private Transform cardVisual;
    [SerializeField] public Image highlight;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHoverHand = 1.5f;
    [SerializeField] private float scaleOnHoverField = 1.25f;
    [SerializeField] private float verticalMoveOnHover = 110;
    [SerializeField] private float scaleTransition = .5f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;

    [Header("Swap Parameters")]
    [SerializeField] private bool swapAnimations = true;
    [SerializeField] private float swapRotationAngle = 30;
    [SerializeField] private float swapTransition = .15f;
    [SerializeField] private int swapVibrato = 5;

    [Header("Curve")]
    [SerializeField] private CurveParameters curve;

    [Header("Shadow Shape")]
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private Sprite cardFrameSprite;

    [Header("Card UI Items")]
    [SerializeField] private GameObject cardFrontPrefab;
    [SerializeField] private GameObject cardBackPrefab;

    public GameObject cardFront;

    private float curveYOffset;
    private float curveRotationOffset;
    private Coroutine pressCoroutine;

    public void Initialize(GameCard target, int index = 0)
    {
        //Declarations
        parentCard = target;
        cardTransform = target.transform;
        canvas = GetComponent<Canvas>();

        //Event Listening
        parentCard.PointerEnterEvent.AddListener(PointerEnter);
        parentCard.PointerExitEvent.AddListener(PointerExit);
        parentCard.BeginDragEvent.AddListener(BeginDrag);
        parentCard.EndDragEvent.AddListener(EndDrag);

        //UI
        GameObject cardFront = Instantiate(cardBackPrefab, cardVisual);
        shadowImage.sprite = cardBackSprite;

        //Initialization
        initalize = true;
    }

    public void SetCardData(CardSO cardSO)
    {
        foreach (Transform transform in cardVisual)
        {
            Destroy(transform.gameObject);
        }

        cardFront = Instantiate(cardFrontPrefab, cardVisual);
        cardFront.GetComponent<CardUI>().SetCardData(cardSO);
        shadowImage.sprite = cardFrameSprite;
    }

    public void UpdateCardData(Card card)
    {
        CardUI cardUI = GetComponentInChildren<CardUI>();
        if (cardUI != null)
        {
            cardUI.UpdateCardData(card);
        }
    }

    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }

    void Update()
    {
        if (!initalize || parentCard == null) return;

        HandPositioning();
        SmoothFollow();
        FollowRotation();
        CardTilt();

    }

    private void HandPositioning()
    {
        curveYOffset = (curve.positioning.Evaluate(parentCard.NormalizedPosition()) * curve.positioningInfluence) * parentCard.SiblingAmount();
        curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : curveYOffset;
        curveRotationOffset = curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }

    private void SmoothFollow()
    {
        Vector3 verticalOffset = (Vector3.up * (parentCard.isDragging ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = (transform.position - cardTransform.position);
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void CardTilt()
    {
        savedIndex = parentCard.isDragging ? savedIndex : parentCard.ParentIndex();
        float sine = Mathf.Sin(Time.time + savedIndex) * (parentCard.isHovering ? .2f : 1);
        float cosine = Mathf.Cos(Time.time + savedIndex) * (parentCard.isHovering ? .2f : 1);

        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane));
        float tiltX = parentCard.isHovering ? ((offset.y * -1) * manualTiltAmount) : 0;
        float tiltY = parentCard.isHovering ? ((offset.x) * manualTiltAmount) : 0;
        float tiltZ = parentCard.isDragging ? tiltParent.eulerAngles.z : (curveRotationOffset * (curve.rotationInfluence * parentCard.SiblingAmount()));

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    public void Swap(float dir = 1)
    {
        if (!swapAnimations)
            return;

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation((Vector3.forward * swapRotationAngle) * dir, swapTransition, swapVibrato, 1).SetId(3);
    }

    private void BeginDrag(GameCard card)
    {
        shakeParent.DOLocalMoveY(0, scaleTransition);
        canvas.overrideSorting = true;
    }

    private void EndDrag(GameCard card)
    {
        shakeParent.DOLocalMoveY(0, scaleTransition);

        canvas.overrideSorting = false;
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void PointerEnter(GameCard card)
    {
        if (card.isInHand && card.isMine)
        {
            shakeParent.DOLocalMoveY(1 * verticalMoveOnHover, scaleTransition);
        }

        canvas.overrideSorting = card.isMine;
        if (scaleAnimations && !(!card.isMine && card.isInHand))
            transform.DOScale(card.isInHand ? scaleOnHoverHand : scaleOnHoverField, scaleTransition).SetEase(scaleEase);

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void PointerExit(GameCard card)
    {
        shakeParent.DOLocalMoveY(0, scaleTransition);

        canvas.overrideSorting = false;
        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

}
