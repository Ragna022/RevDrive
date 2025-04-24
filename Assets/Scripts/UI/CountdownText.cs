using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownText : MonoBehaviour
{
    public Text countdownText; // drag your Text UI element here
    public float delayBetweenSteps = 1f;
    public float scaleUp = 1.5f;

    private string[] countdownSequence = { "READY!", "1", "2", "3", "GO!" };
    public PrometeoCarController carController;

    void Start()
    {
        // Ensure the car controller is disabled at the start
        if (carController != null)
        {
            carController.enabled = false;
        }
        else
        {
            Debug.LogWarning("Car Controller reference not set in CountdownText script.");
        }

        StartCoroutine(PlayCountdown());
    }

    public IEnumerator PlayCountdown()
    {
        countdownText.gameObject.SetActive(true);

        foreach (string step in countdownSequence)
        {
            countdownText.text = step;
            countdownText.transform.localScale = Vector3.zero;

            // Animate scale in
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

        // Enable the car controller after the countdown finishes
        if (carController != null)
        {
            carController.enabled = true;
        }

        // Optional: trigger something after countdown
        // e.g., carController.enabled = true;
    }
}
