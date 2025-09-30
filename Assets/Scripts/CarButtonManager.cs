using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CarButtonManager : MonoBehaviour
{
    [Header("Car Configuration")]
    public List<CarConfig> carConfigs = new List<CarConfig>();

    [System.Serializable]
    public class CarConfig
    {
        public string carId;
        public string linkedNFTMintAddress;
        public string carName;
    }

    private SimpleNFTManager nftManager;
    private bool initialized = false;

    void Start()
    {
        // Find NFT manager
        nftManager = FindObjectOfType<SimpleNFTManager>();

        // Ensure event system exists
        if (CarSelectionEvents.Instance == null)
        {
            GameObject eventSystemObj = new GameObject("CarSelectionEvents");
            eventSystemObj.AddComponent<CarSelectionEvents>();
        }

        // Register all cars with the event system
        RegisterAllCars();

        // Initially set all cars as locked
        SetAllCarsLocked();

        // Check NFT ownership every 2 seconds
        InvokeRepeating(nameof(CheckNFTOwnership), 2f, 2f);
    }

    void RegisterAllCars()
    {
        foreach (var carConfig in carConfigs)
        {
            CarSelectionEvents.Instance.RegisterCar(carConfig.carId, carConfig.linkedNFTMintAddress);
        }
    }

    void SetAllCarsLocked()
    {
        foreach (var carConfig in carConfigs)
        {
            CarSelectionEvents.Instance.UpdateCarOwnership(carConfig.carId, false);
        }
    }

    void CheckNFTOwnership()
    {
        if (nftManager == null || CarSelectionEvents.Instance == null)
        {
            Debug.Log("Waiting for managers to initialize...");
            return;
        }

        var ownedNFTs = nftManager.GetDetectedNFTs();

        // Create a list of owned mint addresses
        List<string> ownedMintAddresses = new List<string>();
        foreach (var nft in ownedNFTs)
        {
            ownedMintAddresses.Add(nft.mintAddress);
            Debug.Log($"Found owned NFT: {nft.mintAddress}");
        }

        // Update ownership for each car
        foreach (var carConfig in carConfigs)
        {
            bool isOwned = ownedMintAddresses.Contains(carConfig.linkedNFTMintAddress);
            CarSelectionEvents.Instance.UpdateCarOwnership(carConfig.carId, isOwned);
        }

        if (ownedNFTs.Count > 0 && !initialized)
        {
            initialized = true;
            Debug.Log($"Car system initialized. Found {ownedNFTs.Count} owned NFT cars.");
        }
    }

    // Public method to manually refresh ownership
    public void RefreshOwnership()
    {
        CheckNFTOwnership();
    }

    // Optional: Handle car selection events
    void OnEnable()
    {
        if (CarSelectionEvents.Instance != null)
        {
            CarSelectionEvents.Instance.onCarSelected.AddListener(OnCarSelected);
        }
    }

    void OnDisable()
    {
        if (CarSelectionEvents.Instance != null)
        {
            CarSelectionEvents.Instance.onCarSelected.RemoveListener(OnCarSelected);
        }
    }

    void OnCarSelected(string carId)
    {
        Debug.Log($"Car selected from any scene: {carId}");

        // Find the car config
        var carConfig = carConfigs.Find(c => c.carId == carId);
        if (carConfig != null)
        {
            // Here you can handle what happens when a car is selected
            // For example: load a scene, enable a car, etc.
            PlayerPrefs.SetString("SelectedCar", carId);
            Debug.Log($"Saved selected car: {carId}");
        }
    }

    public void TestScene()
    {
        SceneManager.LoadScene(1);
    }
}