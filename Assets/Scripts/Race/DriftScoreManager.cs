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
            hudDriftScoreText.text = "Drift: " + Mathf.FloorToInt(totalScore);
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
        currentPopup = Instantiate(popupPrefab, carController.transform.position + Vector3.up * 2f, Quaternion.identity);
        currentPopup.Attach(carController.transform);
    }

    void EndDriftPopup()
    {
        currentPopup.EndPopup();
        currentPopup = null;
    }
}
