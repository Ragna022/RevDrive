using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShrink : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float shrinkAmount = 0.9f; // Adjust for desired shrink level
    public float animationDuration = 0.1f;
    private bool isScaled = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isScaled) // Only scale if not already scaled
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(originalScale * shrinkAmount));
            isScaled = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isScaled) // Only reset if it was scaled
        {
            StopAllCoroutines();
            StartCoroutine(ScaleButton(originalScale));
            isScaled = false;
        }
    }

    private void OnDisable()
    {
        // Reset the scale immediately when the GameObject is set inactive
        transform.localScale = originalScale;
        isScaled = false; // Reset the state
        StopAllCoroutines(); // Ensure no ongoing animations interfere
    }

    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        float time = 0;
        Vector3 initialScale = transform.localScale;

        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}

