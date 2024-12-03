using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using GameModel;
using System;
using DG.Tweening;
using Game.Data;
using Unity.VisualScripting;
using Game.Logic.Actions;
using Game.Logic;
using Unity.Netcode;
using Game.Multiplayer;
using UnityEngine.InputSystem;
using System.Linq;
using GameCore.Events;

public class GameCard : NetworkBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
    public int GameID;

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

        ClientCardManager.Instance.OnSnapshotRegistered += SetCardData;
        ClientCardManager.Instance.OnSnapshotUpdated += UpdateCardData;
    }

    void Update() => stateMachine.Update();
    public void OnBeginDrag(PointerEventData eventData) => stateMachine.HandleBeginDrag(eventData);
    public void OnDrag(PointerEventData eventData) => stateMachine.HandleDrag(eventData);
    public void OnEndDrag(PointerEventData eventData) => stateMachine.HandleEndDrag(eventData);
    public void OnPointerEnter(PointerEventData eventData) => stateMachine.HandlePointerEnter(eventData);
    public void OnPointerExit(PointerEventData eventData) => stateMachine.HandlePointerExit(eventData);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {

        }
        else
        {

        }
    }

    public void FlipCard()
    {
        isMine = false;
        cardVisual.FlipCard();
    }

    public void SetCardData(CardDataSnapshot cardData)
    {
        if (cardData.GameID != GameID) return;

        if (ShouldISeeThisCard(this, cardData))
            cardVisual.SetCardData(cardData);
    }

    public void UpdateCardData(CardDataSnapshot previousData, CardDataSnapshot newData)
    {
        if ((previousData.GameID != GameID) || (newData.GameID != GameID)) return;

        if (newData.CurrentState == CardStateType.Discarded)
        {
            cardVisual.UpdateState(newData.CurrentState);
            DiscardCard();
            return;
        }

        if (isMine) {
            cardVisual.UpdateState(newData.CurrentState);
        }

        if (ShouldISeeThisCard(this, newData)) {
            if (ShouldISeeThisCard(this, previousData))
            {
                cardVisual.UpdateCardData(newData);
            }
            else
            {
                cardVisual.SetCardData(newData);
            }
        }
    }

    public void DiscardCard()
    {
        stateMachine.SetState(new UIDiscardedState(this));
        EventManager.TriggerEvent(GameEventsEnum.CardDied, this);
    }

    public bool ShouldISeeThisCard(GameCard gameCard, CardDataSnapshot cardData) =>
        (gameCard.isMine &&
            (cardData.GameID == gameCard.GameID)) ||
        ((cardData.GameID == gameCard.GameID) &&
            (cardData.CurrentState != CardStateType.InHand) &&
            (cardData.CurrentState != CardStateType.InDeck));

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

    public void ShowInDetailsSlot()
    {
        Transform detailsSlot = GameObject.FindWithTag("CardDetails").transform;

        if (detailsSlot != null)
        {
            ClearDetailsSlot();
            if (cardVisual != null && cardVisual.cardFront != null)
            {
                GameObject cardFront = Instantiate(cardVisual.cardFront, detailsSlot);
                cardFront.transform.DOScale(2, 0);
            }
        }
    }

    public void ClearDetailsSlot()
    {
        Transform detailsSlot = GameObject.FindWithTag("CardDetails").transform;

        if (detailsSlot != null)
        {
            foreach(Transform t in detailsSlot)
            {
                Destroy(t.gameObject);
            }
        }
    }

    private void PlayCardEffect(bool isToDestroy)
    {
        Transform effectSlot = GameObject.FindWithTag("CardEffect").transform;
        if (effectSlot != null)
        {
            if (cardVisual != null && cardVisual.cardFront != null)
            {
                GameObject cardFront = Instantiate(cardVisual.cardFront, effectSlot);
                cardFront.transform.DOScale(2, 0);

                StartCoroutine(WaitSeconds(1f, cardFront));
            }
        }

        if (isToDestroy)
        {
            HorizontalCardHolder horizontalCardHolder = GetComponentInParent<HorizontalCardHolder>();
            horizontalCardHolder.RemoveCard(this);
            cardVisual.gameObject.SetActive(false);
        }

        IEnumerator WaitSeconds(float seconds, GameObject cardFront)
        {
            yield return new WaitForSecondsRealtime(seconds); 
            if (effectSlot != null)
            {
                Destroy(cardFront);
            }

            if (isToDestroy)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public void PlayCardOnBoard(GameObject playerBoardArea)
    {
        HorizontalCardHolder horizontalCardHolder = GetComponentInParent<HorizontalCardHolder>();
        horizontalCardHolder.RemoveCard(this);
        horizontalCardHolder.UpdateIndexes();

        RectTransform cardSlotTransform = cardTransform.parent as RectTransform;
        cardSlotTransform.SetParent(playerBoardArea.transform);

        horizontalCardHolder = GetComponentInParent<HorizontalCardHolder>();
        horizontalCardHolder.AddCard(this);
        horizontalCardHolder.UpdateIndexes();
        isInHand = false;
    }

    private void PlayCard(GameObject playerBoardArea)
    {
        CardDataSnapshot cardSnapshot = ClientCardManager.Instance.GetCardSnapshotByGameId(GameID);

        switch (cardSnapshot.CardType)
        {
            case CardType.BattleTactic:
                //TRY PLAY BATTLE TACTIC
                // if can't play card, don't run the code below
               
                //PlayCardEffect(true);
                break;
            case CardType.Unit:
                ActionRequestHandler.Instance.HandlePlayCardRequestServerRpc(GameID);
                break;
            case CardType.Legend:
                ActionRequestHandler.Instance.HandlePlayCardRequestServerRpc(GameID);
                //PlayCardEffect(false);
                return;
        }
    }

    private void AttackCard(GameObject targetCard)
    {
        targetCard.TryGetComponent(out GameCard target);

        try
        {
            ActionRequestHandler.Instance.HandleAttackCardRequestServerRpc(GameID, target.GameID, NetworkManager.Singleton.LocalClientId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void AttackOpponent(GameObject targetOpponent)
    {
        Debug.Log("Attacking opponent");

        try
        {
            // TODO: Should get playerID from "targetOpponent" but for now, there are only 2 players so to save time we are just going to get the other player's ID from the NetworkManager

            ulong clientID = NetworkManager.Singleton.LocalClientId;
            ulong targetPlayerID = NetworkManager.Singleton.ConnectedClientsIds.First((id) => id != clientID);

            ActionRequestHandler.Instance.HandleAttackMythRequestServerRpc(clientID, targetPlayerID, GameID);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
