using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Vector3[] spawnPoints; // Assign manually in inspector for each car
    [SerializeField] private Vector3 spawnRotationEuler; // Set this once to make all cars face front

    void Start()
    {
        var carPrefab = CarSelectionManager.Instance.selectedCarPrefab;
        var color = CarSelectionManager.Instance.selectedCarColor;

        if (carPrefab != null)
        {
            int selectedIndex = GetSelectedCarIndex(carPrefab);
            Vector3 spawnPosition = Vector3.zero;

            if (selectedIndex >= 0 && selectedIndex < spawnPoints.Length)
                spawnPosition = spawnPoints[selectedIndex];
            else
                Debug.LogWarning("Selected index is out of range of spawnPoints. Defaulting to Vector3.zero.");

            Quaternion spawnRotation = Quaternion.Euler(spawnRotationEuler);
            GameObject car = Instantiate(carPrefab, spawnPosition, spawnRotation);

            // Apply the color to the car's body
            var renderers = car.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                if (r.material.name.Contains("Body"))
                    r.material.color = color;
            }
        }
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
