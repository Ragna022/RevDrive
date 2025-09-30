using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("UI References")]
    public Image speedometerFillImage; // This replaces the needle GameObject
    [Space(10)]
    [Header("Turbo Effect")]
    public GameObject turboEffectGameObject; // Drag the GameObject to enable/disable here
=======
    public PrometeoCarController carController;
    public Image speedometerFillImage;
>>>>>>> Stashed changes

    private float startPosition = 212f, endPosition = -35.7f;
    private float desiredPosition;
    private float vehicleSpeed;
    private PrometeoCarController carController;

<<<<<<< Updated upstream
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
=======
    [Space(10)]
    [Header("Turbo Effect GameObject")]
    public GameObject turboEffectGameObject;

    void Start()
    {
        // Automatically find the car controller
        FindCarController();
        
>>>>>>> Stashed changes
        if (turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(false);
        }
    }

<<<<<<< Updated upstream
    void Update()
=======
    void FindCarController()
>>>>>>> Stashed changes
    {
        if (carController == null)
        {
            carController = FindObjectOfType<PrometeoCarController>();
            if (carController == null)
            {
                Debug.LogError("GameManager: No PrometeoCarController found in scene!");
            }
            else
            {
                Debug.Log("GameManager: Automatically found PrometeoCarController");
            }
        }
    }

<<<<<<< Updated upstream
        // Control the turbo effect GameObject's active state
        if (carController != null && turboEffectGameObject != null)
=======
    void Update()
    {
        // Ensure we have a reference to the car controller
        if (carController == null)
        {
            FindCarController();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            carController.ActivateTurbo();
        }

        if (turboEffectGameObject != null)
>>>>>>> Stashed changes
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
            float normalizedSpeed = Mathf.Clamp01(vehicleSpeed / 180f);
            speedometerFillImage.fillAmount = normalizedSpeed;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
}