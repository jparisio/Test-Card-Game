using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PlayPile : MonoBehaviour
{
    public PlayPile instance;
    public HorizontalLayoutGroup layoutGroup;
    public RectTransform container;
    public List<Card> cards = new List<Card>();

    void Start()
    {
        instance = this;
        container = GetComponent<RectTransform>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    void Update()
    {
        if(cards.Count > 0){
            foreach (Card card in cards)
            {
                card.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        if(GameManager.instance.state == GameState.CountPlayedCards)
        {
            StartCoroutine(CountPlayedCards());
        }
    }

    private IEnumerator CountPlayedCards()
    {
        //wait like just a tad before counting 
        yield return new WaitForSeconds(.1f);
        int index = 0;
        foreach (Card card in cards)
        {
            DOTween.Kill(index, true);
            card.cardvisual.rotation.DOPunchRotation(Vector3.forward * 20f, .2f, 30, 1).SetId(index);
            card.cardvisual.transform.DOPunchScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f, 2, 1).SetId(index);
            index++;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(.1f);

        StartCoroutine(DiscardCards());
    }


    private IEnumerator DiscardCards()
    {
        foreach (Card card in cards)
        {
            DiscardPile discardPile = FindFirstObjectByType<DiscardPile>();
            card.transform.parent.SetParent(discardPile.transform);
            yield return new WaitForSeconds(0.03f);
        }
        GameManager.instance.UpdateGameState(GameState.DestroyCards);
        DestroyCards();
    }

    private void DestroyCards()
    {
        foreach (Card card in cards)
        {
            Destroy(card.cardvisual.gameObject);
            Destroy(card.transform.parent.gameObject);
        }
        cards.Clear();
        GameManager.instance.UpdateGameState(GameState.DrawCards);
    }
}
