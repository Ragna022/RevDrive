using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public Image speedometerFillImage; // This replaces the needle GameObject
    [Space(10)]
    [Header("Turbo Effect")]
    public GameObject turboEffectGameObject; // Drag the GameObject to enable/disable here

    private float startPosition = 212f, endPosition = -35.7f;
    private float desiredPosition;
    private float vehicleSpeed;
    private PrometeoCarController carController;

    void Start()
    {
        // Find the player vehicle by tag
        GameObject playerVehicle = GameObject.FindGameObjectWithTag("Player");
        
        if (playerVehicle == null)
        {
            Debug.LogError("No GameObject with 'Player' tag found in the scene!");
            enabled = false; // Disable the script if no player found
            return;
        }

        carController = playerVehicle.GetComponent<PrometeoCarController>();
        
        if (carController == null)
        {
            Debug.LogError("No PrometeoCarController component found on the player vehicle!");
            enabled = false; // Disable the script if no controller found
            return;
        }

        // Optional: Ensure the turbo effect GameObject is initially off
        if (turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(false);
        }
    }

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

        // Control the turbo effect GameObject's active state
        if (carController != null && turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(carController.IsTurboActive);
        }
    }

    void FixedUpdate()
    {
        if (carController != null)
        {
            vehicleSpeed = carController.carSpeed;
            UpdateSpeedometerFill();
        }
    }

    void UpdateSpeedometerFill()
    {
        if (speedometerFillImage != null)
        {
            // Assuming maximum speed is 180, normalize between 0 and 1
            float normalizedSpeed = Mathf.Clamp01(vehicleSpeed / 180f);
            speedometerFillImage.fillAmount = normalizedSpeed;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
}