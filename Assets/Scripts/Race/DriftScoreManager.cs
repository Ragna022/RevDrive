using UnityEngine;
using UnityEngine.UI;

public class DriftScoreManager : MonoBehaviour
{
    [Header("Drift Settings")]
    public float pointsPerSecond = 10f;

    [Header("UI References")]
    public Text hudDriftScoreText;
    public DriftPopup popupPrefab;

    private float totalScore = 0f;
    private float currentCombo = 0f;
    private DriftPopup currentPopup;
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

        // Initialize HUD text
        if (hudDriftScoreText != null)
        {
            hudDriftScoreText.text = "Drift: 0";
        }
    }

    void Update()
    {
        if (carController == null) return;

        if (carController.isDrifting)
        {
            if (currentPopup == null)
            {
                StartDriftPopup();
            }

            currentCombo += pointsPerSecond * Time.deltaTime;
            totalScore += pointsPerSecond * Time.deltaTime;

            currentPopup.UpdatePopup(Mathf.FloorToInt(currentCombo));
            
            if (hudDriftScoreText != null)
            {
                hudDriftScoreText.text = "Drift: " + Mathf.FloorToInt(totalScore);
            }
        }
        else
        {
            if (currentPopup != null)
            {
                EndDriftPopup();
            }

            currentCombo = 0f; // Reset combo when not drifting
        }
    }

    void StartDriftPopup()
    {
        if (popupPrefab == null || carController == null) return;
        
        currentPopup = Instantiate(popupPrefab, carController.transform.position + Vector3.up * 2f, Quaternion.identity);
        currentPopup.Attach(carController.transform);
    }

    void EndDriftPopup()
    {
        if (currentPopup != null)
        {
            currentPopup.EndPopup();
            currentPopup = null;
        }
    }
}