using UnityEngine;
using System;
using System.Collections.Generic;

public class AICarController : MonoBehaviour
{
    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public enum Axel
    {
        Front,
        Rear
    }

    [Header("Car Physics")]
    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;
    public Vector3 _centerOfMass;
    public List<Wheel> wheels;

    [Header("AI Settings")]
    public Transform[] waypoints;
    public float lookAheadDistance = 10f;
    public float maxSpeed = 50f;
    public float brakeDistance = 15f;
    public float obstacleAvoidanceRange = 8f;
    public LayerMask obstacleLayer = 1;
    
    [Header("Lane Settings")]
    public float laneWidth = 8f;
    public float laneOffset = 0f; // Will be set automatically by race manager
    
    [Header("AI Personality")]
    [Range(0.5f, 1.5f)]
    public float aggressiveness = 1f; // Affects speed and risk-taking
    [Range(0.1f, 2f)]
    public float skill = 1f; // Affects precision and reaction time
    
    // Internal AI variables
    private int currentWaypoint = 0;
    private Vector3 targetPosition;
    private float moveInput;
    private float steerInput;
    private Rigidbody carRb;
    private float currentSpeed;
    private bool isObstacleDetected = false;
    
    // Racing state
    public int racePosition = 1;
    public float lapTime = 0f;
    public int currentLap = 0;
    public bool raceFinished = false;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        
        if (waypoints.Length > 0)
        {
            targetPosition = waypoints[0].position;
        }
    }

    void Update()
    {
        if (!raceFinished)
        {
            UpdateAI();
            AnimateWheels();
            UpdateRaceStats();
        }
    }

    void LateUpdate()
    {
        if (!raceFinished)
        {
            Move();
            Steer();
            Brake();
        }
    }

    void UpdateAI()
    {
        currentSpeed = carRb.linearVelocity.magnitude * 3.6f; // Convert to km/h
        
        // Update waypoint navigation
        UpdateWaypoint();
        
        // Calculate steering and movement
        CalculateSteering();
        CalculateMovement();
        
        // Obstacle avoidance
        AvoidObstacles();
    }

    void UpdateWaypoint()
    {
        if (waypoints.Length == 0) return;
        
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);
        
        if (distanceToWaypoint < lookAheadDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
                currentLap++;
            }
        }
        
        // Look ahead for smoother cornering
        int lookAheadWaypoint = (currentWaypoint + 1) % waypoints.Length;
        Vector3 currentWP = waypoints[currentWaypoint].position;
        Vector3 nextWP = waypoints[lookAheadWaypoint].position;
        
        Vector3 baseTarget = Vector3.Lerp(currentWP, nextWP, 
            Mathf.Clamp01(distanceToWaypoint / lookAheadDistance));
        
        // Add lane offset for spacing
        Vector3 direction = (nextWP - currentWP).normalized;
        Vector3 rightVector = Vector3.Cross(direction, Vector3.up).normalized;
        
        targetPosition = baseTarget + (rightVector * laneOffset);
    }

    void CalculateSteering()
    {
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition);
        float steerAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        
        // Apply skill factor (less skilled drivers are less precise)
        steerAngle += UnityEngine.Random.Range(-5f, 5f) / skill;
        
        steerInput = Mathf.Clamp(steerAngle / maxSteerAngle, -1f, 1f);
        
        // Smooth steering based on skill
        steerInput = Mathf.Lerp(steerInput, steerInput, skill);
    }

    void CalculateMovement()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float targetSpeed = maxSpeed * aggressiveness;
        
        // Slow down for turns
        float steerAmount = Mathf.Abs(steerInput);
        if (steerAmount > 0.3f)
        {
            targetSpeed *= (1f - steerAmount * 0.5f);
        }
        
        // Brake before sharp turns
        if (steerAmount > 0.6f && currentSpeed > targetSpeed)
        {
            moveInput = -0.5f; // Brake
        }
        else if (currentSpeed < targetSpeed && !isObstacleDetected)
        {
            moveInput = 1f; // Accelerate
        }
        else
        {
            moveInput = 0.5f; // Maintain speed
        }
        
        // Apply skill-based variations
        moveInput *= UnityEngine.Random.Range(0.8f, 1.2f) * skill;
        moveInput = Mathf.Clamp(moveInput, -1f, 1f);
    }

    void AvoidObstacles()
    {
        isObstacleDetected = false;
        
        // Check for obstacles ahead
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        RaycastHit hit;
        
        // Multiple forward raycasts for better detection
        Vector3[] rayDirections = {
            forward,
            forward + right * 0.3f,
            forward - right * 0.3f
        };
        
        float avoidanceStrength = 0f;
        
        foreach (Vector3 rayDir in rayDirections)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, rayDir.normalized, out hit, obstacleAvoidanceRange, obstacleLayer))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    isObstacleDetected = true;
                    
                    // Calculate avoidance direction
                    Vector3 avoidDir = Vector3.Cross(rayDir, Vector3.up);
                    if (Vector3.Dot(right, hit.point - transform.position) > 0)
                    {
                        avoidanceStrength -= 0.8f; // Steer left
                    }
                    else
                    {
                        avoidanceStrength += 0.8f; // Steer right
                    }
                    
                    // Reduce speed when avoiding
                    moveInput *= 0.6f;
                }
            }
        }
        
        // Apply avoidance steering
        if (isObstacleDetected)
        {
            steerInput += avoidanceStrength;
            steerInput = Mathf.Clamp(steerInput, -1f, 1f);
        }
        
        // Side detection for lane changing
        CheckSideObstacles();
    }
    
    void CheckSideObstacles()
    {
        Vector3 right = transform.right;
        float sideCheckDistance = 5f;
        
        // Check left and right for potential overtaking
        if (Physics.Raycast(transform.position, right, sideCheckDistance, obstacleLayer))
        {
            // Obstacle on right, prefer left
            laneOffset = Mathf.Lerp(laneOffset, -laneWidth * 0.5f, Time.deltaTime * 2f);
        }
        else if (Physics.Raycast(transform.position, -right, sideCheckDistance, obstacleLayer))
        {
            // Obstacle on left, prefer right
            laneOffset = Mathf.Lerp(laneOffset, laneWidth * 0.5f, Time.deltaTime * 2f);
        }
        else
        {
            // No side obstacles, gradually return to original lane
            laneOffset = Mathf.Lerp(laneOffset, laneOffset * 0.5f, Time.deltaTime * 1f);
        }
    }

    void Move()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        bool shouldBrake = moveInput < 0 || isObstacleDetected;
        
        if (shouldBrake)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void UpdateRaceStats()
    {
        if (!raceFinished)
        {
            lapTime += Time.deltaTime;
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;
        
        // Draw waypoint path
        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 2f);
                
                int nextIndex = (i + 1) % waypoints.Length;
                if (waypoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                }
            }
        }
        
        // Draw current target
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 1f);
        
        // Draw obstacle detection rays
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleAvoidanceRange);
        Gizmos.DrawRay(transform.position, (transform.forward + transform.right * 0.5f).normalized * obstacleAvoidanceRange * 0.7f);
        Gizmos.DrawRay(transform.position, (transform.forward - transform.right * 0.5f).normalized * obstacleAvoidanceRange * 0.7f);
    }
}