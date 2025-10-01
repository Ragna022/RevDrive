using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Misc : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup pausePanel; // Background fade
    [SerializeField] private RectTransform pauseSlate; // Pop-up window

    [Header("Animation Speeds")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scaleUpDuration = 0.4f;
    [SerializeField] private float scaleDownDuration = 0.3f;

    [Header("Scale Settings")]
    [SerializeField] private float overshootScale = 1.1f;

    private Sequence openSequence, closeSequence;

    void Start()
    {
        pausePanel.alpha = 0f;
        pausePanel.blocksRaycasts = false;
        pauseSlate.localScale = Vector3.zero;
    }

    public void OpenPauseMenu()
    {
        DOTween.KillAll();

        pausePanel.blocksRaycasts = true;

        openSequence = DOTween.Sequence();

        openSequence
            // Fade in panel
            .Append(pausePanel.DOFade(1f, fadeDuration))
            
            // Start scaling up just before fade ends (e.g., 80% of fade)
            .Insert(fadeDuration * 0.8f,
                pauseSlate.DOScale(overshootScale, scaleUpDuration * 0.7f)
                .SetEase(Ease.OutCubic))
            
            .Insert(fadeDuration * 0.8f + scaleUpDuration * 0.7f,
                pauseSlate.DOScale(1f, scaleUpDuration * 0.3f)
                .SetEase(Ease.OutBack));
    }

    public void ClosePauseMenu()
    {
        DOTween.KillAll();

        closeSequence = DOTween.Sequence();

        closeSequence
            // Pop down first
            .Append(pauseSlate.DOScale(0f, scaleDownDuration).SetEase(Ease.InBack))

            // Then fade out panel
            .Append(pausePanel.DOFade(0f, fadeDuration))
            .OnComplete(() =>
            {
                pausePanel.blocksRaycasts = false;
            });
    }
}

