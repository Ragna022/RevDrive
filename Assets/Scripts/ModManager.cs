using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModManager : MonoBehaviour
{
    [SerializeField] private GameObject[] carModels; // These are already placed in scene
    [SerializeField] private GameObject[] carPrefabs; // These are used in the gameplay scene
    [SerializeField] private Color[] colorOptions;
    [SerializeField] private GameObject[] hoverIcons;
    [SerializeField] public Material bodyMaterial;
    [SerializeField] private Text carNameLabel;    
    public AudioClip click;          
    public AudioSource audioSource;

    private int currentCarIndex = 0;

    void Start()
    {
        for (int i = 0; i < carModels.Length; i++)
            carModels[i].SetActive(false);

        CarSelectionManager.Instance.allCarPrefabs = carPrefabs;

        ShowCarModels(0);
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

    public void ChangeColour(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= colorOptions.Length) return;

        Color selectedColor = colorOptions[colorIndex];
        bodyMaterial.color = selectedColor;

        CarSelectionManager.Instance.selectedCarColor = selectedColor;
    }

    public void ShowHoverIcon(int hoverIndex)
    {
        if (hoverIndex < 0 || hoverIndex >= hoverIcons.Length) return;
        
        foreach(var obj in hoverIcons)
        {
            if(obj == hoverIcons[hoverIndex])
            {
                obj.SetActive(true);
            }
              else
            {
                obj.SetActive(false);
            }
        }
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

