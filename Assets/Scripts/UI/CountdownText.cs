using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownText : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("UI Settings")]
    public Text countdownText; // drag your Text UI element here
=======
    public Text countdownText;
>>>>>>> Stashed changes
    public float delayBetweenSteps = 1f;
    public float scaleUp = 1.5f;

    private string[] countdownSequence = { "READY!", "1", "2", "3", "GO!" };
<<<<<<< Updated upstream
    private PrometeoCarController carController;

    void Start()
    {
        // Find the player vehicle by tag
        GameObject playerVehicle = GameObject.FindGameObjectWithTag("Player");
        
        if (playerVehicle == null)
        {
            Debug.LogError("No GameObject with 'Player' tag found in the scene!");
            return;
        }

        carController = playerVehicle.GetComponent<PrometeoCarController>();
        
        if (carController == null)
        {
            Debug.LogError("No PrometeoCarController component found on the player vehicle!");
            return;
        }

        // Ensure the car controller is disabled at the start
        carController.enabled = false;
        StartCoroutine(PlayCountdown());
=======
    public PrometeoCarController carController;
    
    public System.Action OnCountdownComplete;

    void Start()
    {
        // Automatically find the car controller
        FindCarController();
        countdownText.gameObject.SetActive(false);
    }

    void FindCarController()
    {
        if (carController == null)
        {
            carController = FindObjectOfType<PrometeoCarController>();
            if (carController == null)
            {
                Debug.LogError("CountdownText: No PrometeoCarController found in scene!");
            }
            else
            {
                Debug.Log("CountdownText: Automatically found PrometeoCarController");
            }
        }
>>>>>>> Stashed changes
    }

    public IEnumerator PlayCountdown()
    {
<<<<<<< Updated upstream
        if (countdownText == null)
        {
            Debug.LogError("Countdown Text reference not set!");
            yield break;
=======
        // Ensure we have a reference before starting
        if (carController == null)
        {
            FindCarController();
>>>>>>> Stashed changes
        }

        countdownText.gameObject.SetActive(true);

        foreach (string step in countdownSequence)
        {
            countdownText.text = step;
            countdownText.transform.localScale = Vector3.zero;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 3f;
                countdownText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * scaleUp, t);
                yield return null;
            }

            yield return new WaitForSeconds(delayBetweenSteps);
        }

        countdownText.gameObject.SetActive(false);
<<<<<<< Updated upstream

        // Enable the car controller after the countdown finishes
        if (carController != null)
        {
            carController.enabled = true;
        }
    }
=======
        OnCountdownComplete?.Invoke();
    }

    public void StartCountdown()
    {
        StartCoroutine(PlayCountdown());
    }
>>>>>>> Stashed changes
}