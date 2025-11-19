using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarCarousel : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    public GameObject dotPrefab;
    [SerializeField] private Transform dotContainer;
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = Color.gray;
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private Vector3 normalScale;
    public float lerpSpeed = 10f;

    [SerializeField] private Text carNameText;
    public ModManager modManager;
    public Button useCarButton;

    [SerializeField] private Slider accelerationSlider;
    [SerializeField] private Slider topSpeedSlider;
    [SerializeField] private Slider handlingSlider;
    [SerializeField] private Slider nitroSlider;

    private List<RectTransform> items = new List<RectTransform>();
    private List<GameObject> dots = new List<GameObject>();
    private RectTransform viewport;
    private int currentIndex = -1;

    void Start()
    {
        viewport = scrollRect.viewport;

        int itemCount = transform.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            RectTransform item = transform.GetChild(i).GetComponent<RectTransform>();
            items.Add(item);

            GameObject dot = Instantiate(dotPrefab, dotContainer);
            dots.Add(dot);
        }

        UpdateDotIndicators(0);
        SelectCar(0); // Show first car by default
    }

    void Update()
    {
        RectTransform closestItem = GetClosestToViewportCenter(out int index);

        for (int i = 0; i < items.Count; i++)
        {
            bool isCenter = items[i] == closestItem;
            Vector3 targetScale = isCenter ? normalScale * scaleFactor : normalScale;
            items[i].localScale = Vector3.Lerp(items[i].localScale, targetScale, Time.deltaTime * lerpSpeed);

            CarCarouselItem item = items[i].GetComponent<CarCarouselItem>();
            if (item != null)
            {
                // Always show locked image if car is locked
                item.lockedImage.SetActive(item.isLocked);
            }
        }

        // 🔧 Update UI when the center car changes
        if (index != currentIndex)
        {
            currentIndex = index;
            UpdateDotIndicators(index);
            SelectCar(index);
        }
    }

    RectTransform GetClosestToViewportCenter(out int index)
    {
        float closestDistance = float.MaxValue;
        RectTransform closest = null;
        index = 0;

        Vector3 centerWorld = viewport.TransformPoint(viewport.rect.center);

        for (int i = 0; i < items.Count; i++)
        {
            
            float distance = Mathf.Abs(items[i].position.x - centerWorld.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = items[i];
                index = i;
            }
        }

        return closest;
    }

    void UpdateDotIndicators(int activeIndex)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            Image dotImage = dots[i].GetComponent<Image>();
            dotImage.color = (i == activeIndex) ? activeDotColor : inactiveDotColor;
        }
    }

    public void SelectCar(int index)
    {
        if (index < 0 || index >= items.Count) return;

        currentIndex = index;

        CarCarouselItem item = items[index].GetComponent<CarCarouselItem>();
        if (item == null) return;

        carNameText.text = item.carName;
        useCarButton.interactable = !item.isLocked;

        accelerationSlider.value = item.acceleration;
        topSpeedSlider.value = item.topSpeed;
        handlingSlider.value = item.handling;
        nitroSlider.value = item.nitro;

        modManager.ShowCarModels(index);
    }
    

    public void OnCarButtonPressed(int index)
    {
        SelectCar(index); // This updates sliders, car name, and play button state
    }

    public void OnUseCarButtonPressed()
    {
        CarCarouselItem selectedCarItem = items[currentIndex].GetComponent<CarCarouselItem>();

        if (selectedCarItem != null && !selectedCarItem.isLocked)
        {
            // Set the selected car prefab in the CarSelectionManager
            CarSelectionManager.Instance.selectedCarPrefab = 
                CarSelectionManager.Instance.allCarPrefabs[currentIndex];
        }
    }

    public void UnlockCurrentCar()
    {
        CarCarouselItem item = items[currentIndex].GetComponent<CarCarouselItem>();

        if (item != null)
        {
            item.isLocked = false;
            item.lockedImage.SetActive(false);
            useCarButton.interactable = true;
        }
    }
}
