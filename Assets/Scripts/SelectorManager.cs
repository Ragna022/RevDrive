using UnityEngine;
using TMPro;

public class SelectorManager : MonoBehaviour
{
    public GameObject[] cars;
    public TextMeshProUGUI carNameLabel; // 👈 Drag your TMP text here
    private int currentIndex = 0;
    public AudioSource audioSource;
    public AudioClip click;

    void Start()
    {
        UpdateCarDisplay();
    }

    public void SelectNext()
    {
        currentIndex = (currentIndex + 1) % cars.Length;
        UpdateCarDisplay();

        if (currentIndex >= cars.Length)
        {
            currentIndex = cars.Length;
            Debug.LogWarning("Reached End");
        }
    }

    public void SelectPrevious()
    {
        currentIndex = (currentIndex - 1 + cars.Length) % cars.Length;
        UpdateCarDisplay();

        if (currentIndex == 0)
        {
            currentIndex = 0;
            Debug.LogWarning("Beginning");
        }
    }

    private void UpdateCarDisplay()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(i == currentIndex);
        }

        if (carNameLabel != null)
        {
            carNameLabel.text = cars[currentIndex].name;
        }
    }

    public int GetSelectedIndex() => currentIndex;
    public GameObject GetSelectedCar() => cars[currentIndex];

    public void ClickSound()
    {
        audioSource.PlayOneShot(click);
    }
}

