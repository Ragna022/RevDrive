using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manage : MonoBehaviour
{
    public RectTransform[] buttons;         
    public float flyInSpeed = 0.5f, yShootPos, delayBetween = 0.2f;   
    public Vector2 offscreenOffset = new Vector2(830, 0); 
    public AudioClip swooshSound, click;          
    public AudioSource audioSource;

    [Header("Loading Screen")]
    [SerializeField] private GameObject HomeMenu, loadingUI;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText, loadingMessage;

    void Start()
    {
        AnimateButton();
    }

        void AnimateButton()
    {
        Sequence sequence = DOTween.Sequence();
        float initialDelay = 0.1f; // Delay before first button animates (prevents instant swoosh)

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform btn = buttons[i];
            Vector2 originalPos = btn.anchoredPosition;
            Vector2 overshootPos = originalPos - new Vector2(0, yShootPos);

            // Start offscreen
            btn.anchoredPosition = originalPos + offscreenOffset;

            float buttonDelay = initialDelay + i * delayBetween;

            // Animate to overshoot (with swoosh)
            sequence.Insert(buttonDelay, btn.DOAnchorPos(overshootPos, flyInSpeed * 0.7f)
                .SetEase(Ease.OutCubic)
                .OnStart(() =>
                {
                    if (swooshSound && audioSource)
                        audioSource.PlayOneShot(swooshSound);
                }));

            // Animate bounce-back
            sequence.Insert(buttonDelay + (flyInSpeed * 0.7f), btn.DOAnchorPos(originalPos, flyInSpeed * 0.3f)
                .SetEase(Ease.OutBack));
        }

        sequence.Play();
    }



    // Game Mode Buttons

    public void QuickRace()
    {
        swapUI();
        StartCoroutine(LoadSceneAsync("QuickRace"));
    }

    public void TimeTrial()
    {
        swapUI();
        StartCoroutine(LoadSceneAsync("TimeTrial"));
    }

    public void DriftSprint()
    {
        swapUI();
        StartCoroutine(LoadSceneAsync("Drift"));
    }

    public void LoadGarageScene()
    {  
        swapUI();
        StartCoroutine(LoadGarageSceneAsync("GarageScene"));
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(click);
    }
    
    void swapUI()
    {
        loadingUI.SetActive(true);
        HomeMenu.SetActive(false);
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        string[] fakeMessages = {
            "Can't rush greatness...",
            "Here goes nothing..!",
            "Your chance, don't blow it mate...",
            "Fueling the fury...",
            "Summoning adrenaline surge...",
            "Testing brakes, we need you alive...",
            "Gear up, driver..!",
            "Vooom time, your manners please...",
            "Grip. Grit. Go.",
            "Fasten your seatbelt - we're rewriting physics.",
            "Tuning engines, breaking hearts...",
            "Just a pit stop before greatness.",
            "Breathe...",
            "Patience is a virtue, ha..",
            "The fun never stops, does it?",
            "Loading..."
        };

        progressBar.value = 0f;
        progressText.text = "0%";

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        loadingMessage.text = fakeMessages[Random.Range(0, fakeMessages.Length)];

        yield return new WaitForSeconds(Random.Range(0.8f, 1.8f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float displayProgress = 0f;

        while (!asyncLoad.isDone)
        {
            float targetProgress = asyncLoad.progress;
            if (targetProgress >= 0.9f) targetProgress = 1f;

            displayProgress = Mathf.Lerp(displayProgress, targetProgress, Time.deltaTime * 3f);
            progressBar.value = displayProgress;
            progressText.text = Mathf.RoundToInt(displayProgress * 100) + "%";

            if (Random.value < 0.02f)
                loadingMessage.text = fakeMessages[Random.Range(0, fakeMessages.Length)];

            if (displayProgress >= 0.98f)
            {
                loadingMessage.text = "Finalizing Reality...";
                yield return new WaitForSeconds(Random.Range(1f, 2f));
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Once the scene has loaded, spawn the car
        SpawnCar();
    }

    void SpawnCar()
    {
        // Check if the car prefab is set
        GameObject selectedCarPrefab = CarSelectionManager.Instance.selectedCarPrefab;
        if (selectedCarPrefab != null)
        {
            // Find the spawn point for the current scene
            Transform spawnPoint = GetSpawnPointForCurrentScene();

            // Instantiate the car at the correct spawn point
            if (spawnPoint != null)
            {
                Instantiate(selectedCarPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    Transform GetSpawnPointForCurrentScene()
    {
        // You can create a system to dynamically get the spawn point for each scene, like:
        string sceneName = SceneManager.GetActiveScene().name;

        // Return appropriate spawn point based on the scene name
        switch (sceneName)
        {
            case "QuickRace":
                return GameObject.Find("QuickRaceSpawnPoint")?.transform;
            case "TimeTrial":
                return GameObject.Find("TimeTrialSpawnPoint")?.transform;
            case "Drift":
                return GameObject.Find("DriftSpawnPoint")?.transform;
            default:
                return null;
        }
    }

    IEnumerator LoadGarageSceneAsync(string sceneName)
    {
        string[] fakeMessages = {
            "Next stop - Garage..!",
            "Wanna change some colours?...",
            "Someone prolly needs a new toy...",
            "Gear up time..!",
            "Car's striking a pose... don't keep it waiting.",
            "Your dream ride is doing a little twirl",
            "One sec, prepping the showroom for greatness.",
            "Loading style, speed, and that one colour you'll never pick.",
            "Painting pixels and inflating polygons...",
            "Configuring 'Vroom Vroom' settings...",
            "Car's loading its best angle. Be patient, it's a shy thing.",
            "Please hold",
            "Patience is a virtue, ha.."
        };

        progressBar.value = 0f;
        progressText.text = "0%";

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        loadingMessage.text = fakeMessages[Random.Range(0, fakeMessages.Length)];

        yield return new WaitForSeconds(Random.Range(0.8f, 1.8f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float displayProgress = 0f;

        while (!asyncLoad.isDone)
        {
            float targetProgress = asyncLoad.progress;
            if (targetProgress >= 0.9f) targetProgress = 1f;

            displayProgress = Mathf.Lerp(displayProgress, targetProgress, Time.deltaTime * 3f);
            progressBar.value = displayProgress;
            progressText.text = Mathf.RoundToInt(displayProgress * 100) + "%";

            if (Random.value < 0.02f)
                loadingMessage.text = fakeMessages[Random.Range(0, fakeMessages.Length)];

            if (displayProgress >= 0.98f)
            {
                loadingMessage.text = "Finalizing Reality...";
                yield return new WaitForSeconds(Random.Range(1f, 2f));
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
