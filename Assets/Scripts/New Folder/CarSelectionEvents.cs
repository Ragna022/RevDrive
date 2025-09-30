using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CarSelectionEvents : MonoBehaviour
{
    public static CarSelectionEvents Instance;
    
    [System.Serializable]
    public class CarButtonEvent : UnityEvent<string, bool> { }
    
    public CarButtonEvent onCarButtonStateChange = new CarButtonEvent();
    public UnityEvent<string> onCarSelected = new UnityEvent<string>();
    
    private Dictionary<string, bool> carOwnershipStates = new Dictionary<string, bool>();
    private Dictionary<string, string> carIdToMintAddress = new Dictionary<string, string>();
    
    // Track active listeners to prevent memory leaks
    private List<UnityAction<string, bool>> activeListeners = new List<UnityAction<string, bool>>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("CarSelectionEvents system initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnDestroy()
    {
        // Clean up all listeners when destroyed
        foreach (var listener in activeListeners)
        {
            onCarButtonStateChange.RemoveListener(listener);
        }
        activeListeners.Clear();
    }
    
    public void RegisterCar(string carId, string mintAddress)
    {
        if (!carIdToMintAddress.ContainsKey(carId) && !string.IsNullOrEmpty(carId))
        {
            carIdToMintAddress[carId] = mintAddress;
            Debug.Log($"Registered car: {carId} -> {mintAddress}");
        }
    }
    
    public void UpdateCarOwnership(string carId, bool isOwned)
    {
        if (string.IsNullOrEmpty(carId)) return;
        
        carOwnershipStates[carId] = isOwned;
        
        // Safe invocation with null check
        if (onCarButtonStateChange != null)
        {
            onCarButtonStateChange.Invoke(carId, isOwned);
        }
        
        Debug.Log($"Car {carId} ownership updated: {isOwned}");
    }
    
    public bool IsCarOwned(string carId)
    {
        return !string.IsNullOrEmpty(carId) && 
               carOwnershipStates.ContainsKey(carId) && 
               carOwnershipStates[carId];
    }
    
    public string GetMintAddressForCar(string carId)
    {
        return !string.IsNullOrEmpty(carId) && carIdToMintAddress.ContainsKey(carId) ? 
               carIdToMintAddress[carId] : null;
    }
    
    public void SelectCar(string carId)
    {
        if (string.IsNullOrEmpty(carId)) return;
        
        if (IsCarOwned(carId) && onCarSelected != null)
        {
            onCarSelected.Invoke(carId);
            Debug.Log($"Car selected: {carId}");
        }
        else
        {
            Debug.LogWarning($"Cannot select car {carId} - not owned");
        }
    }
    
    public void ClearAllOwnership()
    {
        carOwnershipStates.Clear();
        Debug.Log("Cleared all car ownership states");
    }
    
    // Safe method to add listeners
    public void SafeAddListener(UnityAction<string, bool> listener)
    {
        if (listener != null && !activeListeners.Contains(listener))
        {
            onCarButtonStateChange.AddListener(listener);
            activeListeners.Add(listener);
        }
    }
    
    // Safe method to remove listeners
    public void SafeRemoveListener(UnityAction<string, bool> listener)
    {
        if (listener != null && activeListeners.Contains(listener))
        {
            onCarButtonStateChange.RemoveListener(listener);
            activeListeners.Remove(listener);
        }
    }
}