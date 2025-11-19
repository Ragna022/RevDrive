using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PrometeoCarController carController;
    public Image speedometerFillImage;

    private float startPosition = 212f, endPosition = -35.7f;
    private float desiredPosition;
    public float vehicleSpeed;

    [Space(10)]
    [Header("Turbo Effect GameObject")]
    public GameObject turboEffectGameObject;

    [System.Obsolete]
    void Start()
    {
        // Automatically find the car controller
        StartCoroutine(FindCarController());
        
        if (turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(false);
        }
    }

    [System.Obsolete]
    IEnumerator FindCarController()
    {
        yield return new WaitForSeconds(1);

        if (carController == null)
        {
            carController = FindObjectOfType<PrometeoCarController>();
            
            if (carController == null)
            {
                Debug.LogError("GameManager: No PrometeoCarController found in scene!");
            }
            else
            {
                Debug.Log("GameManager: Automatically found PrometeoCarController");
            }
        }
    }

    [System.Obsolete]
    void Update()
    {
        // Ensure we have a reference to the car controller
        if (carController == null)
        {
            FindCarController();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            carController.ActivateTurbo();
        }

        if (turboEffectGameObject != null)
        {
            turboEffectGameObject.SetActive(carController.IsTurboActive);
        }
    }

    void FixedUpdate()
    {
        if (carController != null)
        {
            vehicleSpeed = carController.carSpeed;
            UpdateSpeedometerFill();
        }
    }

    void UpdateSpeedometerFill()
    {
        if (speedometerFillImage != null)
        {
            float normalizedSpeed = Mathf.Clamp01(vehicleSpeed / 180f);
            speedometerFillImage.fillAmount = normalizedSpeed;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("Home");
    }
}