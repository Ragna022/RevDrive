using UnityEngine;
using UnityEngine.UI;

public class DriftScoreManager : MonoBehaviour
{
    [Header("Drift Settings")]
    public float pointsPerSecond = 10f;

    [Header("References")]
    public PrometeoCarController carController;
    public Text hudDriftScoreText;
    public DriftPopup popupPrefab;

    private float totalScore = 0f;
    private float currentCombo = 0f;
    private DriftPopup currentPopup;

    void Start()
    {
        // Automatically find the car controller
        FindCarController();
        
        // Find HUD text if not assigned
        if (hudDriftScoreText == null)
        {
            hudDriftScoreText = GameObject.FindGameObjectWithTag("DriftScore")?.GetComponent<Text>();
        }
    }

    void FindCarController()
    {
        if (carController == null)
        {
            carController = FindObjectOfType<PrometeoCarController>();
            if (carController == null)
            {
                Debug.LogError("DriftScoreManager: No PrometeoCarController found in scene!");
            }
            else
            {
                Debug.Log("DriftScoreManager: Automatically found PrometeoCarController");
            }
        }
    }

    void Update()
    {
        if (carController == null)
        {
            FindCarController();
            return;
        }

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
            currentCombo = 0f;
        }
    }

    void StartDriftPopup()
    {
        if (popupPrefab != null && carController != null)
        {
            currentPopup = Instantiate(popupPrefab, carController.transform.position + Vector3.up * 2f, Quaternion.identity);
            currentPopup.Attach(carController.transform);
        }
    }

    void EndDriftPopup()
    {
        currentPopup.EndPopup();
        currentPopup = null;
    }
}