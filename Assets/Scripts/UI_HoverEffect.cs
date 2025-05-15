using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class UI_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleUp = 1.1f;
    public float tweenTime = 0.2f;
    public Color hoverColor;

    private Vector3 originalScale;
    private Color originalColor;
    private Image img;
    public AudioClip hoverSound;          
    public AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        img = GetComponent<Image>();
        originalColor = img.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * scaleUp, tweenTime).SetEase(Ease.OutBack);
        img.DOColor(hoverColor, tweenTime);
        audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, tweenTime).SetEase(Ease.InBack);
        img.DOColor(originalColor, tweenTime);
    }
}
