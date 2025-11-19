using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModManager : MonoBehaviour
{
    [SerializeField] private GameObject[] carModels; // Car models on stand
    [SerializeField] private GameObject[] carPrefabs; // Car prefabs to be spawned in gameplay scene
    [SerializeField] private Text carNameLabel;
    public AudioClip click;
    public AudioSource audioSource;

    private int currentCarIndex = 0;

    void Start()
    {
        for (int i = 0; i < carModels.Length; i++)
            carModels[i].SetActive(false);

        CarSelectionManager.Instance.allCarPrefabs = carPrefabs;

        foreach (var model in carModels)
            model.SetActive(false);
    }

    public void ShowCarModels(int index)
    {
        if (index < 0 || index >= carModels.Length) return;

        foreach (var model in carModels)
        model.SetActive(false);

        currentCarIndex = index;
        carModels[currentCarIndex].SetActive(true);

        carNameLabel.text = carModels[currentCarIndex].name;

        CarSelectionManager.Instance.selectedCarPrefab = carPrefabs[index];
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene("Home");
    }

    public void PlayClickSound()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && click != null)
            audioSource.PlayOneShot(click);
    }
}
