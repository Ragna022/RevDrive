using UnityEngine;
using UnityEngine.UI;

public class DriftPopup : MonoBehaviour
{
    public Text comboText; // Assign in prefab (world space canvas with Text component)
    private Transform followTarget;
    private CanvasGroup canvasGroup;

    private Vector3 worldOffset;
    private float currentScale = 0.003f; // Reduced starting size
    private const float maxScale = 0.005f; // Reduced max size

    private bool isEnding = false;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Offset in camera space, then convert to world space once
        float radius = 1.5f;
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 camOffset = Camera.main.transform.right * randomCircle.x + Camera.main.transform.up * randomCircle.y;
        worldOffset = camOffset;

        transform.localScale = Vector3.one * currentScale;
    }

    public void Attach(Transform target)
    {
        followTarget = target;
        transform.position = followTarget.position + worldOffset;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }

    public void UpdatePopup(int value)
    {
        if (comboText != null)
            comboText.text = value + " pts";

        currentScale = Mathf.Min(currentScale + Time.deltaTime * 1f, maxScale);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * currentScale, Time.deltaTime * 8f);

        canvasGroup.alpha = Mathf.Min(canvasGroup.alpha + Time.deltaTime * 4f, 1f);
    }

    public void EndPopup()
    {
        if (!isEnding)
        {
            isEnding = true;
            StartCoroutine(FadeAndDestroy());
        }
    }

    System.Collections.IEnumerator FadeAndDestroy()
    {
        float fadeDuration = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (followTarget != null)
        {
            // Follow the target smoothly
            Vector3 targetPos = followTarget.position + worldOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);

            // Lock rotation to face the camera only once per frame (stable)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        }
    }
}
