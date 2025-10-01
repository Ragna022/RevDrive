using UnityEngine;
using UnityEngine.UI;

public class NitroBarController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundFill;
    [SerializeField] private Image foregroundFill;
    
    [Header("Colors")]
    [SerializeField] private Color readyColor = new Color(0.1f, 0.6f, 1f);
    [SerializeField] private Color activeColor = new Color(0f, 0.8f, 1f);
    [SerializeField] private Color chargingColor = new Color(0.2f, 0.2f, 0.5f);
    [SerializeField] private Color driftColor = new Color(1f, 0.6f, 0f);
    
    [Header("Effects")]
    [SerializeField] private float fillSmoothness = 10f;
    [SerializeField] private float pulseSpeed = 1.5f;
    
    private PrometeoCarController carController;

    private void Start()
    {
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
        }
    }
    
    private void Update()
    {
        // Re-find if reference was lost
        if (carController == null)
        {
            FindCarController();
            return;
        }
        
        UpdateNitroVisuals();
    }
    
    private void UpdateNitroVisuals()
    {
        float nitroPercent = carController.NitroPercent;
        
        float targetBackgroundFill = nitroPercent;
        backgroundFill.fillAmount = Mathf.Lerp(
            backgroundFill.fillAmount,
            targetBackgroundFill,
            fillSmoothness * Time.deltaTime
        );
        
        foregroundFill.fillAmount = Mathf.Lerp(
            foregroundFill.fillAmount,
            1 - targetBackgroundFill,
            fillSmoothness * Time.deltaTime
        );
        
        UpdateBarColors(nitroPercent);
    }
    
    private void UpdateBarColors(float nitroPercent)
    {
        if(carController.IsTurboActive)
        {
            backgroundFill.color = activeColor;
            foregroundFill.color = new Color(activeColor.r, activeColor.g, activeColor.b, 0.5f);
        }
        else if(nitroPercent >= carController.minUseThreshold)
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 0.2f) + 0.8f;
            Color pulseColor = readyColor * pulse;
            
            backgroundFill.color = carController.isDrifting ? 
                Color.Lerp(driftColor, pulseColor, 0.7f) :
                pulseColor;
                
            foregroundFill.color = new Color(0.3f, 0.3f, 0.6f, 0.4f);
        }
        else
        {
            backgroundFill.color = carController.isDrifting ? 
                driftColor : 
                chargingColor;
                
            foregroundFill.color = new Color(0.2f, 0.2f, 0.4f, 0.4f);
        }
    }
}