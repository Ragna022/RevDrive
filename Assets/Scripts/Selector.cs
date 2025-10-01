using UnityEngine;
using UnityEngine.SceneManagement;

public class Selector : MonoBehaviour
{
    public GameObject[] modPanels;
    public GameObject[] UIPanels;
    public AudioSource audioSource;
    public AudioClip click;

    public void ShowModPanel(int index)
    {
        foreach(var panel in modPanels) 
        {
            panel.SetActive(false);
        }

        modPanels[index].SetActive(true);  // Turn on selected one

        foreach(var UI in UIPanels) 
        {
            UI.SetActive(false);
        }

        UIPanels[index].SetActive(true);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(click);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modPanels[0].SetActive(true);
        UIPanels[0].SetActive(true);
    }

    public void LoadHome()
    {
        SceneManager.LoadScene("Home"); 
    }

    public void OnPlayPressed()
    {
        SceneManager.LoadScene("GamePlayScene"); 
    }
}
