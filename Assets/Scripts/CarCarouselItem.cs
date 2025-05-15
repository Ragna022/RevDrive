using UnityEngine;

public class CarCarouselItem : MonoBehaviour
{
    public GameObject[] goldOverlays;
    public GameObject[] whiteBases;
    public GameObject lockedImage;

    public string carName;
    public bool isLocked = true; // Default to locked

    [Range(1, 100)] public int acceleration;
    [Range(1, 100)] public int topSpeed;
    [Range(1, 100)] public int handling;
    [Range(1, 100)] public int nitro;
}
