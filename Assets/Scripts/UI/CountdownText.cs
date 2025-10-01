using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownText : MonoBehaviour
{
    public Text countdownText;
    public float delayBetweenSteps = 1f;
    public float scaleUp = 1.5f;

    private string[] countdownSequence = { "READY!", "1", "2", "3", "GO!" };
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
    }

    public IEnumerator PlayCountdown()
    {
        // Ensure we have a reference before starting
        if (carController == null)
        {
            FindCarController();
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
        OnCountdownComplete?.Invoke();
    }

    public void StartCountdown()
    {
        StartCoroutine(PlayCountdown());
    }
}