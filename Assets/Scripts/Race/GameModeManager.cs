using UnityEngine;

public enum RaceMode
{
    QuickRace,
    TimeTrial,
    DriftChallenge
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public RaceMode SelectedMode { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMode(RaceMode mode)
    {
        SelectedMode = mode;
        Debug.Log("Selected Mode: " + mode);
    }
}
