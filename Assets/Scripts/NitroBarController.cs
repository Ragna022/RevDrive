using UnityEngine;
using UnityEngine.UI;

public class NitroBarController : MonoBehaviour
{
    [Header("UI References")]
<<<<<<< Updated upstream
    [SerializeField] private Image backgroundFill;  // Shows remaining nitro (decreases during use)
    [SerializeField] private Image foregroundFill; // Shows consumed nitro (increases during use)
    
    [Header("Colors")]
    [SerializeField] private Color readyColor = new Color(0.1f, 0.6f, 1f);      // Bright blue when available
    [SerializeField] private Color activeColor = new Color(0f, 0.8f, 1f);       // Cyan during turbo
    [SerializeField] private Color chargingColor = new Color(0.2f, 0.2f, 0.5f); // Dark blue when charging
    [SerializeField] private Color driftColor = new Color(1f, 0.6f, 0f);        // Orange during drift gains
=======
    [SerializeField] private Image backgroundFill;
    [SerializeField] private Image foregroundFill;
    
    [Header("Colors")]
    [SerializeField] private Color readyColor = new Color(0.1f, 0.6f, 1f);
    [SerializeField] private Color activeColor = new Color(0f, 0.8f, 1f);
    [SerializeField] private Color chargingColor = new Color(0.2f, 0.2f, 0.5f);
    [SerializeField] private Color driftColor = new Color(1f, 0.6f, 0f);
>>>>>>> Stashed changes
    
    [Header("Effects")]
    [SerializeField] private float fillSmoothness = 10f;
    [SerializeField] private float pulseSpeed = 1.5f;
    
    private PrometeoCarController carController;

    private void Start()
    {
<<<<<<< Updated upstream
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
=======
        FindCarController();
    }
    
    void FindCarController()
    {
        if (carController == null)
        {
            // Method 1: Find by tag (preferred if you tag your player)
            GameObject playerVehicle = GameObject.FindGameObjectWithTag("Player");
            if (playerVehicle != null)
            {
                carController = playerVehicle.GetComponent<PrometeoCarController>();
            }
            
            // Method 2: Find any instance in scene (fallback)
            if (carController == null)
            {
                carController = FindObjectOfType<PrometeoCarController>();
            }
            
            if (carController == null)
            {
                Debug.LogError("NitroBarController: No PrometeoCarController found in scene!");
                enabled = false;
            }
            else
            {
                Debug.Log("NitroBarController: Automatically found PrometeoCarController");
            }
>>>>>>> Stashed changes
        }
    }
    
    private void Update()
    {
<<<<<<< Updated upstream
=======
        // Re-find if reference was lost
        if (carController == null)
        {
            FindCarController();
            return;
        }
        
>>>>>>> Stashed changes
        UpdateNitroVisuals();
    }
    
    private void UpdateNitroVisuals()
    {
<<<<<<< Updated upstream
        if (carController == null) return;
        
        float nitroPercent = carController.NitroPercent;
        
        // BACKGROUND shows remaining nitro (1.0 when full, 0.0 when empty)
=======
        float nitroPercent = carController.NitroPercent;
        
>>>>>>> Stashed changes
        float targetBackgroundFill = nitroPercent;
        backgroundFill.fillAmount = Mathf.Lerp(
            backgroundFill.fillAmount,
            targetBackgroundFill,
            fillSmoothness * Time.deltaTime
        );
        
<<<<<<< Updated upstream
        // FOREGROUND shows consumed nitro (inverse of background)
=======
>>>>>>> Stashed changes
        foregroundFill.fillAmount = Mathf.Lerp(
            foregroundFill.fillAmount,
            1 - targetBackgroundFill,
            fillSmoothness * Time.deltaTime
        );
        
<<<<<<< Updated upstream
        // Color management
=======
>>>>>>> Stashed changes
        UpdateBarColors(nitroPercent);
    }
    
    private void UpdateBarColors(float nitroPercent)
    {
<<<<<<< Updated upstream
        if (carController == null) return;
        
        if(carController.IsTurboActive)
        {
            // Turbo active - cyan glow
=======
        if(carController.IsTurboActive)
        {
>>>>>>> Stashed changes
            backgroundFill.color = activeColor;
            foregroundFill.color = new Color(activeColor.r, activeColor.g, activeColor.b, 0.5f);
        }
        else if(nitroPercent >= carController.minUseThreshold)
        {
<<<<<<< Updated upstream
            // Ready to use - pulsating blue
=======
>>>>>>> Stashed changes
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 0.2f) + 0.8f;
            Color pulseColor = readyColor * pulse;
            
            backgroundFill.color = carController.isDrifting ? 
                Color.Lerp(driftColor, pulseColor, 0.7f) :
                pulseColor;
                
            foregroundFill.color = new Color(0.3f, 0.3f, 0.6f, 0.4f);
        }
        else
        {
<<<<<<< Updated upstream
            // Charging - dark colors
=======
>>>>>>> Stashed changes
            backgroundFill.color = carController.isDrifting ? 
                driftColor : 
                chargingColor;
                
            foregroundFill.color = new Color(0.2f, 0.2f, 0.4f, 0.4f);
        }
    }
}