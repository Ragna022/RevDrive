using UnityEngine;

public class CarSelectionManager : MonoBehaviour
{
    public static CarSelectionManager Instance;

    //? Saved car from user's entry
    public GameObject selectedCarPrefab;
    public GameObject[] allCarPrefabs;

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



    


