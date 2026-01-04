using Febucci.TextAnimatorForUnity;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CardDescription : MonoBehaviour
{
    private TypewriterComponent typewriter;
    private TextMeshProUGUI textMeshPro;
    private string text = "This is a sample card description that will be animated using a typewriter effect.";
    void Start()
    {
        typewriter = GetComponentInChildren<TypewriterComponent>();
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        text = textMeshPro.text;
        transform.localScale = Vector3.zero;
    }

    public void PlayAnimation()
    {
        transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
        typewriter.ShowText(text);
    }

    public void Hide()
    {
        transform.DOScale(0, 0.2f).SetEase(Ease.InBack);
    }
}
