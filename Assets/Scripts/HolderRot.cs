using UnityEngine;

public class HolderRot : MonoBehaviour
{
    public float autoRotateSpeed = 10f;
    public float dragRotateSpeed = 5f;
    public Rect touchArea = new Rect(0.25f, 0.25f, 0.5f, 0.5f); // normalized area (center)

    private bool isDragging = false;
    private float lastX;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif

        if (!isDragging)
        {
            transform.Rotate(0, autoRotateSpeed * Time.deltaTime, 0);
        }
    }

    bool IsWithinTouchArea(Vector2 screenPos)
    {
        Vector2 normalizedPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        return touchArea.Contains(normalizedPos);
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && IsWithinTouchArea(Input.mousePosition))
        {
            isDragging = true;
            lastX = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            float deltaX = Input.mousePosition.x - lastX;
            transform.Rotate(0, -deltaX * dragRotateSpeed * Time.deltaTime, 0);
            lastX = Input.mousePosition.x;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && IsWithinTouchArea(touch.position))
            {
                isDragging = true;
                lastX = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }

            if (isDragging && touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.position.x - lastX;
                transform.Rotate(0, -deltaX * dragRotateSpeed * Time.deltaTime, 0);
                lastX = touch.position.x;
            }
        }
    }
}
