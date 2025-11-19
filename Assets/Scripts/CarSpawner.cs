using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Vector3[] spawnPoints; // Assigned manually in inspector for each car
    [SerializeField] private Vector3[] spawnRotationEuler; // Assigned manually in inspector for each car
    
    //? Default car assuming user did not choose any car before playing game
    [SerializeField] private GameObject defaultCar;

    void Start()
    {
        if (CarSelectionManager.Instance == null)
        {
            SpawnDefault();
            return;
        }

        GameObject carPrefab = CarSelectionManager.Instance.selectedCarPrefab;

        if (carPrefab == null)
        {
            SpawnDefault();
            return;
        }

        int index = GetSelectedCarIndex(carPrefab);

        if (index < 0 || index >= spawnPoints.Length)
        {
            Debug.LogWarning("Invalid index. Spawning default.");
            SpawnDefault();
            return;
        }

        Quaternion rot = Quaternion.Euler(spawnRotationEuler[index]);
        Instantiate(carPrefab, spawnPoints[index], rot);
        Debug.Log("Spawned selected car!");
    }

    private void SpawnDefault()
    {
        Quaternion rot = Quaternion.Euler(spawnRotationEuler[0]);
        Instantiate(defaultCar, spawnPoints[0], rot);
        Debug.Log("Spawned DEFAULT car!");
    }

    private int GetSelectedCarIndex(GameObject selectedPrefab)
    {
        GameObject[] carPrefabs = CarSelectionManager.Instance.allCarPrefabs;
        for (int i = 0; i < carPrefabs.Length; i++)
        {
            if (carPrefabs[i] == selectedPrefab)
                return i;
        }

        return -1;
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene("SelectorScene");
    }
}
