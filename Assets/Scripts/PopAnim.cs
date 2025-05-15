using UnityEngine;

public class PopAnim : MonoBehaviour
{
    [SerializeField] private float popSpeed = 2f;     // Speed of the pulse
    [SerializeField] private float popScale = 1.1f;   // How much bigger it gets
    [SerializeField] private bool randomStartPhase = true;

    private Vector3 originalScale;
    private float timer;

    void Start()
    {
        originalScale = transform.localScale;
        timer = randomStartPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    void Update()
    {
        timer += Time.deltaTime * popSpeed;
        float scale = 1 + (Mathf.Sin(timer) * 0.5f * (popScale - 1));
        transform.localScale = originalScale * scale;
    }
}


