using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public class CardContainer : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private GameObject cardSlot;
    [SerializeField] private int numCards = 5;
    [SerializeField] private int DrawCardCount = 15;
    [SerializeField] private Card draggedCard;
    [SerializeField] private List<Card> selectedCards = new List<Card>();
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private RectTransform container;
    [SerializeField] private AnimationCurve offsetCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    

    void Awake()
    {
        //this is all disbaled when numCards is set to 0 (which it is)
        for (int i = 0; i < numCards; i++)
        {
            GameObject slot = Instantiate(cardSlot, transform);
        }

        cards = GetComponentsInChildren<Card>().ToList();

        int counter = 0;
        foreach (Card card in cards)
        {
            card.OnCardDragStart.AddListener(SetDraggingCard);
            card.OnCardDragEnd.AddListener(ClearDraggingCard);
            card.OnCardClick.AddListener(SelectCard);
            card.name = "Card " + counter;
            counter++;
        }

        GameManager.OnGameStateChanged += HandleGameStateChange;

    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    
    void Update()
    {
        HandleCardDrag();
        AdjustSpacing();
        UpdateCardSortingOrder();

        if (Input.GetKeyDown(KeyCode.D) && GameManager.instance.state == GameState.SelectCards && selectedCards.Count > 0)
        {
            GameManager.instance.UpdateGameState(GameState.PlayCards);
            
        }

        if (Input.GetKeyDown(KeyCode.S) && GameManager.instance.state == GameState.SelectCards)
        {
            SortCardsBySuit();
        }

        if (Input.GetKeyDown(KeyCode.A) && GameManager.instance.state == GameState.SelectCards)
        {
            SortCardsByValue();
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        if(GameManager.instance.state == GameState.DrawCards)
        {
            StartCoroutine(DrawCardsWithDelay(DrawCardCount, 0.05f));
        } else if(GameManager.instance.state == GameState.PlayCards){
            StartCoroutine(PlayCards());
        }
    }

    public void HandleCardDrag()
    {
        if (draggedCard != null)
        {
            foreach (Card card in cards)
            {
                if (card != draggedCard)
                {
                    // Check if dragging card is moving right
                    if (draggedCard.transform.position.x > card.transform.position.x)
                    {
                        if (draggedCard.transform.parent.GetSiblingIndex() < card.transform.parent.GetSiblingIndex())
                        {
                            SwapCards(draggedCard, card);
                            break;
                        }
                    }

                    // Check if dragging card is moving left
                    if (draggedCard.transform.position.x < card.transform.position.x)
                    {
                        if (draggedCard.transform.parent.GetSiblingIndex() > card.transform.parent.GetSiblingIndex())
                        {
                            SwapCards(draggedCard, card);
                            break;
                        }
                    }
                }
            }
        }
    }


    private void SwapCards(Card card1, Card card2)
    {
        // Store the original parents
        Transform originalParent1 = card1.transform.parent;
        Transform originalParent2 = card2.transform.parent;

        // Swap parents without changing world positions
        card1.transform.SetParent(originalParent2, true);
        card2.transform.SetParent(originalParent1, true);
    }

    private void UpdateCardSortingOrder()
    {
        foreach (Card card in cards)
        {
            if (card.cardvisual.canvas != null)
            {
                card.cardvisual.canvas.overrideSorting = true;
                card.cardvisual.canvas.sortingOrder = card.transform.parent.GetSiblingIndex();
            }
            
            ApplyOffset(card);
        }
    }

    private void ApplyOffset(Card card)
    {
        // Apply offsets depending on the card's position relative to the animation curve
        int siblingIndex = card.transform.parent.GetSiblingIndex();
        int cardCount = cards.Count;

        // Handle edge cases for fewer cards
        float normalizedIndex = cardCount > 1 ? siblingIndex / (float)(cardCount - 1) : 0.5f;

        card.yOffset = offsetCurve.Evaluate(normalizedIndex);
        card.rotationOffset = rotationCurve.Evaluate(normalizedIndex);
    }

    private void SetDraggingCard(Card card)
    {
        draggedCard = card;

    }

    private void ClearDraggingCard(Card card)
    {
        if (draggedCard == card)
            draggedCard = null;
    }

    private void SelectCard(Card card)
    {
        if(GameManager.instance.state != GameState.SelectCards)
            return;

        if(card.isDragging) return;

        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.selected = false;
        } else if(selectedCards.Count < 5){
            selectedCards.Add(card);
            card.selected = true;
        }
    }


    private void AdjustSpacing()
    {
        float containerWidth = container.rect.width;

        // Calculate total space available for spacing (container width - total card width)
        float totalCardWidth = cards.Count * 150f;
        float totalSpacing = containerWidth - totalCardWidth;

        // Calculate spacing between cards
        float spacing = (cards.Count > 1) ? totalSpacing / (cards.Count - 1) : 0;

        // If there's not enough space, allow negative spacing to overlap the cards
        layoutGroup.spacing = spacing;
    }

    public void DrawCard(){
        GameObject slot = Instantiate(cardSlot, transform);
        Card card = slot.GetComponentInChildren<Card>();
        card.OnCardDragStart.AddListener(SetDraggingCard);
        card.OnCardDragEnd.AddListener(ClearDraggingCard);
        card.OnCardClick.AddListener(SelectCard);
        card.name = "Card " + cards.Count;
        //choose random suit and value
        Suit suit = (Suit)Random.Range(0, 4);
        int value = Random.Range(2, 11); //exclusive so doesnt include 11
        card.Intitialize(suit, value);
        cards.Add(card);
    }

    public IEnumerator DrawCardsWithDelay(int numberOfCards, float delay)
    {
        int cardsToDraw = numberOfCards - cards.Count;
        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(delay);
        }
        GameManager.instance.UpdateGameState(GameState.SelectCards);
    }

    private IEnumerator PlayCards(){
        foreach(Card card in selectedCards){
            //move to play pile
            PlayPile playPile = FindFirstObjectByType<PlayPile>();
            card.transform.parent.SetParent(playPile.transform);
            card.transform.rotation = Quaternion.Euler(0, 0, 0);
            //remove from cards array 
            cards.Remove(card);
            playPile.cards.Add(card);
            card.enabled = false;
            //wait for a bit
            yield return new WaitForSeconds(0.1f);
        }
        selectedCards.Clear();
        GameManager.instance.UpdateGameState(GameState.CountPlayedCards);
    }

    public void SortCardsBySuit(){
        cards = cards.OrderByDescending(card => card.suit).ThenByDescending(card => card.value).ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(transform.GetChild(i), true);
        }
    }

    public void SortCardsByValue(){
        cards = cards.OrderByDescending(card => card.value).ThenByDescending(card => card.suit).ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(transform.GetChild(i), true);
        }
    }
   
}

