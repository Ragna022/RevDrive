using UnityEngine;

public class CarSelectionManager : MonoBehaviour
{
    public static CarSelectionManager Instance;

    public GameObject selectedCarPrefab;
    public Color selectedCarColor;
    public GameObject[] allCarPrefabs; // Add this in the garage scene

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}



    


