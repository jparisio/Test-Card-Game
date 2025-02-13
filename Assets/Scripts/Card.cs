using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, 
IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerUpHandler
{
    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> OnCardDragStart;
    [HideInInspector] public UnityEvent<Card> OnCardDragEnd;
    [HideInInspector] public UnityEvent<Card> OnCardClick;
    [HideInInspector] public UnityEvent<Card> OnCardHoverEnter;
    [HideInInspector] public UnityEvent<Card> OnCardHoverExit;
    [HideInInspector] public UnityEvent<Card> OnCardPointerUp;

    [Header("States")]
    public bool selected = false;
    public bool isDragging = false;

    [Header("offsets")]
    public float yOffset = 1f;
    public float rotationOffset = 1f;


    [Header("References")]
    [SerializeField] private CardVisual cardVisualPrefab;
    public CardVisual cardvisual;
    private VisualHandler visualHandler;


    [Header("Card Details")]
    public Suit suit;
    public int value;
    public Sprite cardImage;

    public void Intitialize(Suit suit, int value)
    {
        this.suit = suit;
        this.value = value;
        cardImage = Resources.Load<Sprite>($"Sprites/{value}{suit.ToString()}");
        transform.localPosition = Vector3.zero;
        visualHandler = FindFirstObjectByType<VisualHandler>();
        cardvisual = Instantiate(cardVisualPrefab, visualHandler.transform);
        cardvisual.Initialize(this);
    }

    private void Start()
    {
        // transform.localPosition = Vector3.zero;
        // visualHandler = FindFirstObjectByType<VisualHandler>();
        // cardvisual = Instantiate(cardVisualPrefab, visualHandler.transform);
        // cardvisual.Initialize(this);
    }

    private void Update()
    {
        if (!isDragging) // Only apply movement when NOT dragging
        {
            Vector3 targetPos = selected ? new Vector3(0, 50, 0) : Vector3.zero;
            Vector3 targetRot = Vector3.zero;
            
            transform.localPosition = targetPos + new Vector3(0, yOffset * 75f, 0);
            transform.eulerAngles = targetRot + new Vector3(0, 0, rotationOffset * 20f);
            // Debug.Log(transform.localRotation);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        OnCardHoverEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        OnCardHoverExit.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnCardDragStart.Invoke(this);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnCardDragEnd.Invoke(this);
        isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClick.Invoke(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnCardPointerUp.Invoke(this);
    }
}
