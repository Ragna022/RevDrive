using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartQuickRace()
    {
        GameModeManager.Instance.SetMode(RaceMode.QuickRace);
        SceneManager.LoadScene(1);
    }

    public void StartTimeTrial()
    {
        GameModeManager.Instance.SetMode(RaceMode.TimeTrial);
        SceneManager.LoadScene(2);
    }

    public void StartDriftChallenge()
    {
        GameModeManager.Instance.SetMode(RaceMode.DriftChallenge);
        SceneManager.LoadScene(3);
    }
}
