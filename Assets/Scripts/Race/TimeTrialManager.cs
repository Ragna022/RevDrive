using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeTrialManager : MonoBehaviour
{
    public CountdownText countdown;
    public GameObject playerVehicle;
    public Text timerText;
    public GameObject finishUI;
    public Text finalTimeText;
    public Text bestTimeText;
    public AudioSource engineAudioSource;
    public AudioSource driftAudioSource;

    private float timer = 0f;
    private bool raceStarted = false;

    private const string bestTimeKey = "TimeTrialBestTime";

    void Start()
    {
        playerVehicle.GetComponent<PrometeoCarController>().enabled = false;
        finishUI.SetActive(false);
        StartCoroutine(StartTimeTrial());
    }

    void Update()
    {
        if (raceStarted)
        {
            timer += Time.deltaTime;
            timerText.text = FormatTime(timer);
        }
    }

    IEnumerator StartTimeTrial()
    {
        yield return StartCoroutine(countdown.PlayCountdown());

        playerVehicle.GetComponent<PrometeoCarController>().enabled = true;
        raceStarted = true;
    }

    public void FinishRace()
    {
        if (!raceStarted) return;

        raceStarted = false;

        string finalTimeStr = FormatTime(timer);
        finalTimeText.text = "Your Time: " + finalTimeStr;

        float bestTime = PlayerPrefs.GetFloat(bestTimeKey, float.MaxValue);
        if (timer < bestTime)
        {
            PlayerPrefs.SetFloat(bestTimeKey, timer);
            bestTime = timer;
        }

        bestTimeText.text = "Best Time: " + FormatTime(bestTime);

        StartCoroutine(ShowFinishPanelDelayed());
    }

    IEnumerator ShowFinishPanelDelayed()
    {
        Time.timeScale = 0.4f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(0.25f);

        playerVehicle.GetComponent<PrometeoCarController>().enabled = false;

        // Stop all engine sounds
        AudioSource[] audioSources = playerVehicle.GetComponentsInChildren<AudioSource>();
        if (engineAudioSource != null)
            engineAudioSource.mute = true;

        if (driftAudioSource != null)
            driftAudioSource.mute = true;

        Destroy(playerVehicle); // Optional: still destroy it visually

        finishUI.SetActive(true);
    }

    string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);
        int milliseconds = Mathf.FloorToInt((t * 1000) % 1000);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void Home()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(0);
    }

    public void RetryRace()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
