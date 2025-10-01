using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public CountdownText countdown; // reference to your existing countdown script
    public GameObject playerVehicle;
    public Text raceTimerText;
    public GameObject finishUI;

    private float raceTimer;
    private bool raceOngoing = false;

    void Start()
    {
        raceTimer = 0f;
        finishUI.SetActive(false);
        playerVehicle.GetComponent<PrometeoCarController>().enabled = false;
        StartCoroutine(StartRace());
    }

    void Update()
    {
        if (raceOngoing)
        {
            raceTimer += Time.deltaTime;
            raceTimerText.text = FormatTime(raceTimer);
        }
    }

    IEnumerator StartRace()
    {
        yield return StartCoroutine(countdown.PlayCountdown());

        // Enable car
        playerVehicle.GetComponent<PrometeoCarController>().enabled = true;
        raceOngoing = true;
    }

    public void FinishRace()
    {
        if (!raceOngoing) return;

        raceOngoing = false;
        playerVehicle.GetComponent<PrometeoCarController>().enabled = false;
        finishUI.SetActive(true);

        Debug.Log("Race Finished! Time: " + FormatTime(raceTimer));
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
