using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float lerpTime = 3.5f;
    [Range(2f, 3.5f)] public float forwardDistance = 3f;
    public float distance = 2f;

    [Header("Vehicle References")]
    public GameObject attachedVehicle;
    private PrometeoCarController controllerRef;

    [Header("Camera Positions")]
    public Vector2[] cameraPos;

    private int locationIndicator = 0;
    private float accelerationEffect;

    private Transform target;
    private GameObject focusPoint;

    void Start()
    {
        cameraPos = new Vector2[4];
        cameraPos[0] = new Vector2(2f, 0f);
        cameraPos[1] = new Vector2(7.5f, 0.5f);
        cameraPos[2] = new Vector2(8.9f, 1.2f);
        cameraPos[3] = new Vector2(5f, 2f); // optional extra view

        if (attachedVehicle == null)
        {
            Debug.LogError("No attached vehicle assigned!");
            return;
        }

        focusPoint = attachedVehicle.transform.Find("focus")?.gameObject;
        if (focusPoint == null)
        {
            Debug.LogError("No 'focus' object found on attached vehicle!");
            return;
        }

        target = focusPoint.transform;
        controllerRef = attachedVehicle.GetComponent<PrometeoCarController>();
    }

    void FixedUpdate()
    {
        UpdateCam();
    }

    public void CycleCamera()
    {
        locationIndicator++;
        if (locationIndicator >= cameraPos.Length) locationIndicator = 0;
    }

    public void UpdateCam()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleCamera();
        }

        if (target == null || controllerRef == null) return;

        Vector3 offset = (-target.forward * cameraPos[locationIndicator].x) + (target.up * cameraPos[locationIndicator].y);
        Vector3 desiredPosition = target.position + offset;

        accelerationEffect = Mathf.Lerp(accelerationEffect, controllerRef.Gforce * 3.5f, 2 * Time.deltaTime);
        distance = Mathf.Pow(Vector3.Distance(transform.position, desiredPosition), forwardDistance);

        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, distance * Time.deltaTime);

        Transform camChild = transform.GetChild(0);
        if (camChild != null)
        {
            camChild.localRotation = Quaternion.Lerp(camChild.localRotation, Quaternion.Euler(-accelerationEffect * 3f, 0f, 0f), 5f * Time.deltaTime);
        }

        transform.LookAt(target.position);
    }
}
