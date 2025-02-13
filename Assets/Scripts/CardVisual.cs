using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    private Card parentCard;
    private Transform cardTransform;
    public Canvas canvas;
    public Image cardImage;

    [SerializeField]public RectTransform rotation;
    [SerializeField]private RectTransform shake;

    [Header("Animation Settings")]
    private float easeDuration = 0.2f;
    private int shakeStrength = 20;
    private float scaleSize = 1.1f;
    private float lerpSpeed = 30f;
    private Vector3 movementDelta;
    private Vector3 rotationDelta;

    private float rotationSpeed = 25;
    private float rotationAmount = 0.5f;
    private bool resetRotationOnDrag = true;

    public void Initialize(Card card)
    {
        parentCard = card;
        cardTransform = parentCard.transform;
        parentCard.OnCardDragStart.AddListener(OnCardDragStart);
        parentCard.OnCardDragEnd.AddListener(OnCardDragEnd);
        parentCard.OnCardClick.AddListener(OnCardClick);
        parentCard.OnCardHoverEnter.AddListener(PointerEnter);
        parentCard.OnCardHoverExit.AddListener(PointerExit);
        parentCard.OnCardPointerUp.AddListener(OnPointerUp);
        cardImage.sprite = parentCard.cardImage;
        //get components
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        SmoothFollow();
        FollowRotation();
        PlaceDraggedCardOnTop();
    }

    private void SmoothFollow(){
        transform.position = Vector3.Lerp(transform.position, cardTransform.position, Time.deltaTime * lerpSpeed);
        if(!parentCard.isDragging){
            resetRotationOnDrag = true;
            rotation.localRotation = Quaternion.Lerp(rotation.localRotation, cardTransform.rotation, Time.deltaTime * lerpSpeed);
        } else {
            //reset the rotation once when u start dragging so it isnt stuck in the card resting angle
            if(resetRotationOnDrag){
                rotation.localRotation = Quaternion.Lerp(rotation.localRotation, Quaternion.Euler(0, 0, 0), 40f * Time.deltaTime);
                if(Quaternion.Angle(rotation.localRotation, Quaternion.Euler(0,0,0)) < 1f){
                    rotation.localRotation = Quaternion.Euler(0, 0, 0);
                    resetRotationOnDrag = false;
                }
            }
        }
    }

    private void FollowRotation()
    {
        Vector3 movement = (transform.position - cardTransform.position);
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }

    private void PlaceDraggedCardOnTop()
    {
        if(parentCard.isDragging && canvas != null){

            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
        }
    }

    private void OnCardDragStart(Card card)
    {
        
    }

    private void OnCardDragEnd(Card card)
    {
        
    }

    private void OnCardClick(Card card)
    {
        
    }

    private void PointerEnter(Card card)
    {
        
        transform.DOScale(scaleSize, easeDuration).SetEase(Ease.OutBack);

        DOTween.Kill(2, true);
        rotation.DOPunchRotation(Vector3.forward * 5f, easeDuration, shakeStrength, 1).SetId(2);
    }

    private void PointerExit(Card card)
    {
        transform.DOScale(1, .2f).SetEase(Ease.OutBack);
    }

    public void OnPointerUp(Card card)
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

}
