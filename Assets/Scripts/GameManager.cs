using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PrometeoCarController carController;
    public GameObject needle;
    private float startPosition = 212f, endPosition = -35.7f;
    private float desiredPosition;

    public float vehicleSpeed;

    [Space(10)] // Add space in Inspector
    [Header("Turbo Effect GameObject")] // Header for clarity
    public GameObject turboEffectGameObject; // Drag the GameObject to enable/disable here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Optional: Ensure the turbo effect GameObject is initially off
        if (turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for turbo input (e.g., Left Shift key)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (carController != null)
            {
                carController.ActivateTurbo();
            }
        }

        // NEW CODE: Control the turbo effect GameObject's active state
        if (carController != null && turboEffectGameObject != null)
        {
            // Set the active state of the GameObject to match the turbo active state
            turboEffectGameObject.SetActive(carController.IsTurboActive);
        }
    }

    void FixedUpdate()
    {
        // Ensure carController is assigned before accessing its properties
        if (carController != null)
        {
            vehicleSpeed = carController.carSpeed;
            UpdateNeedle();
        }
    }

    void UpdateNeedle()
    {
        desiredPosition = startPosition - endPosition;
        float temp = vehicleSpeed / 180;
        needle.transform.eulerAngles = new Vector3(0, 0,(startPosition - temp * desiredPosition));
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
}