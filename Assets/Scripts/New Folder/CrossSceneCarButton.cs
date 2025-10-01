using UnityEngine;
using UnityEngine.UI;

public class CrossSceneCarButton : MonoBehaviour
{
    [Header("Button Configuration")]
    public string carId = "Car1";
    public string linkedNFTMintAddress = "DXH7mUyy9UEtEwDpNysrjrV4YPP619g41ekDjshAUiNU";
    public string carName = "Sports Car";
    
    [Header("UI References")]
    public Button button;
    public Image lockIcon;
    public Text statusText;
    public Text carNameText;
    
    [Header("Visual States")]
    public Color ownedColor = Color.green;
    public Color lockedColor = Color.gray;
    
    private bool isDestroyed = false;
    
    void Start()
    {
        InitializeButton();
    }
    
    void InitializeButton()
    {
        if (isDestroyed) return;
        
        // Auto-find references if not set
        if (button == null && !TryGetComponent(out button))
        {
            Debug.LogWarning($"Button component not found on {gameObject.name}");
            return;
        }
        
        if (carNameText != null && !string.IsNullOrEmpty(carName))
            carNameText.text = carName;
            
        // Register this car with the event system
        if (CarSelectionEvents.Instance != null)
        {
            CarSelectionEvents.Instance.RegisterCar(carId, linkedNFTMintAddress);
            CarSelectionEvents.Instance.SafeAddListener(OnCarButtonStateChange);
            
            // Set initial state
            bool isOwned = CarSelectionEvents.Instance.IsCarOwned(carId);
            UpdateButtonState(isOwned);
        }
        else
        {
            Debug.LogWarning("CarSelectionEvents instance not found! Will try again.");
            Invoke(nameof(InitializeButton), 1f); // Retry after 1 second
            return;
        }
        
        // Setup click listener
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    void OnCarButtonStateChange(string changedCarId, bool isOwned)
    {
        if (isDestroyed) return;
        
        // Only update if this is our car
        if (changedCarId == carId)
        {
            UpdateButtonState(isOwned);
        }
    }
    
    void UpdateButtonState(bool isOwned)
    {
        if (isDestroyed || button == null) return;
        
        // Check if button still exists
        if (button == null)
        {
            Debug.LogWarning("Button reference lost during update");
            return;
        }
        
        button.interactable = isOwned;
        
        var colors = button.colors;
        if (isOwned)
        {
            colors.normalColor = ownedColor;
            colors.highlightedColor = Color.Lerp(ownedColor, Color.white, 0.3f);
            colors.pressedColor = Color.Lerp(ownedColor, Color.black, 0.3f);
        }
        else
        {
            colors.disabledColor = lockedColor;
        }
        button.colors = colors;
        
        if (lockIcon != null && !isDestroyed)
            lockIcon.gameObject.SetActive(!isOwned);
            
        if (statusText != null && !isDestroyed)
            statusText.text = isOwned ? "OWNED" : "LOCKED";
    }
    
    void OnButtonClicked()
    {
        if (isDestroyed || CarSelectionEvents.Instance == null) return;
        
        CarSelectionEvents.Instance.SelectCar(carId);
    }
    
    void OnDestroy()
    {
        isDestroyed = true;
        
        // Clean up event listeners
        if (CarSelectionEvents.Instance != null)
        {
            CarSelectionEvents.Instance.SafeRemoveListener(OnCarButtonStateChange);
        }
    }
    
    // Public method to manually refresh this button
    public void RefreshButton()
    {
        if (isDestroyed || CarSelectionEvents.Instance == null) return;
        
        bool isOwned = CarSelectionEvents.Instance.IsCarOwned(carId);
        UpdateButtonState(isOwned);
    }
    
    // Safety check for async operations
    public bool IsValid()
    {
        return !isDestroyed && gameObject != null;
    }
}