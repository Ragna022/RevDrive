using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class AIRaceManager : MonoBehaviour
{
    [Header("Race Settings")]
    public int totalLaps = 3;
    public int numberOfAICars = 6;
    public GameObject[] carPrefabs;
    public Transform[] startingPositions;
    public Transform[] waypoints;
    
    [Header("Countdown System")]
    public CountdownText countdownTextScript;
    public PrometeoCarController playerCarController;
    
    [Header("AI Car Settings")]
    public float minAggressiveness = 0.7f;
    public float maxAggressiveness = 1.3f;
    public float minSkill = 0.6f;
    public float maxSkill = 1.4f;
    
    [Header("Lane Spacing")]
    public float trackWidth = 12f;
    public bool randomizeLanes = true;
    
    // Race state
    private List<AICarController> raceCars = new List<AICarController>();
    private bool raceStarted = false;
    private bool raceFinished = false;
    private float raceStartTime;
    private bool countdownActive = true;
    
    // Leaderboard tracking
    private List<RaceResult> finalResults = new List<RaceResult>();
    
    [System.Serializable]
    public class RaceResult
    {
        public string carName;
        public float totalTime;
        public int finalPosition;
        public AICarController carController;
        
        public RaceResult(string name, float time, int position, AICarController controller)
        {
            carName = name;
            totalTime = time;
            finalPosition = position;
            carController = controller;
        }
    }

    void Start()
    {
        SetupRace();
        StartCountdown();
    }

    void Update()
    {
        if (countdownActive)
        {
            // Countdown is handled by CountdownText script
            return;
        }
        else if (raceStarted && !raceFinished)
        {
            UpdateRace();
        }
    }

    void SetupRace()
    {
        // Disable player car at start
        if (playerCarController != null)
        {
            playerCarController.enabled = false;
        }

        // Create AI cars
        for (int i = 0; i < numberOfAICars && i < startingPositions.Length; i++)
        {
            // Select random car prefab or cycle through them
            GameObject selectedCarPrefab = carPrefabs[i % carPrefabs.Length];
            
            // Calculate rotation with -90 degrees offset
            Quaternion spawnRotation = startingPositions[i].rotation * Quaternion.Euler(0, -90, 0);
            
            GameObject carObj = Instantiate(selectedCarPrefab, startingPositions[i].position, spawnRotation);
            AICarController carController = carObj.GetComponent<AICarController>();
            
            if (carController == null)
            {
                carController = carObj.AddComponent<AICarController>();
            }
            
            // Setup waypoints
            carController.waypoints = waypoints;
            
            // Assign lane offset for spacing
            if (randomizeLanes)
            {
                carController.laneOffset = Random.Range(-trackWidth * 0.5f, trackWidth * 0.5f);
            }
            else
            {
                // Distribute cars evenly across track width
                float laneStep = trackWidth / (numberOfAICars - 1);
                carController.laneOffset = (-trackWidth * 0.5f) + (i * laneStep);
            }
            
            // Set lane width for avoidance
            carController.laneWidth = trackWidth / numberOfAICars;
            
            // Randomize AI personality
            carController.aggressiveness = Random.Range(minAggressiveness, maxAggressiveness);
            carController.skill = Random.Range(minSkill, maxSkill);
            
            // Assign car name
            carObj.name = $"AI Car {i + 1} ({selectedCarPrefab.name})";
            
            // Disable car until race starts
            carController.enabled = false;
            
            raceCars.Add(carController);
        }
        
        Debug.Log($"Race setup complete with {raceCars.Count} AI cars with lane spacing");
    }

    void StartCountdown()
    {
        countdownActive = true;
        
        if (countdownTextScript != null)
        {
            // Hook into the countdown completion event
            StartCoroutine(WaitForCountdownCompletion());
        }
        else
        {
            Debug.LogWarning("CountdownText script reference not set! Starting race immediately.");
            StartRace();
        }
    }

    IEnumerator WaitForCountdownCompletion()
    {
        // Wait for the countdown to complete
        yield return StartCoroutine(countdownTextScript.PlayCountdown());
        
        // Countdown finished, start the race
        StartRace();
    }

    void StartRace()
    {
        countdownActive = false;
        raceStarted = true;
        raceStartTime = Time.time;
        
        Debug.Log("GO! Race started!");
        
        // Enable player car
        if (playerCarController != null)
        {
            playerCarController.enabled = true;
        }
        
        // Enable all AI cars
        foreach (var car in raceCars)
        {
            car.enabled = true;
        }
        
        // Optional: Trigger race start events
        OnRaceStart();
    }

    void OnRaceStart()
    {
        // You can add any race start events here
        // For example: play engine sounds, start timers, etc.
        Debug.Log("Race is now live!");
    }

    void UpdateRace()
    {
        // Check for race completion
        int finishedCars = 0;
        
        foreach (var car in raceCars)
        {
            if (car.currentLap >= totalLaps && !car.raceFinished)
            {
                car.raceFinished = true;
                car.racePosition = finishedCars + 1;
                
                float totalTime = Time.time - raceStartTime;
                finalResults.Add(new RaceResult(car.gameObject.name, totalTime, car.racePosition, car));
                
                Debug.Log($"{car.gameObject.name} finished in position {car.racePosition} with time {totalTime:F2}s");
            }
            
            if (car.raceFinished)
            {
                finishedCars++;
            }
        }
        
        // Check if all cars finished
        if (finishedCars >= raceCars.Count)
        {
            EndRace();
        }
    }

    void EndRace()
    {
        raceFinished = true;
        
        Debug.Log("RACE FINISHED!");
        
        // Sort final results
        finalResults = finalResults.OrderBy(result => result.finalPosition).ToList();
        
        Debug.Log("=== FINAL RACE RESULTS ===");
        foreach (var result in finalResults)
        {
            Debug.Log($"{result.finalPosition}. {result.carName} - {result.totalTime:F2}s");
        }
        
        // Optional: Trigger race end events
        OnRaceEnd();
    }

    void OnRaceEnd()
    {
        // You can add any race end events here
        // For example: show results screen, play celebration sounds, etc.
        Debug.Log("Race completed! Showing results...");
    }

    public void RestartRace()
    {
        // Reset race state
        raceStarted = false;
        raceFinished = false;
        countdownActive = true;
        finalResults.Clear();
        
        // Disable player car
        if (playerCarController != null)
        {
            playerCarController.enabled = false;
        }
        
        // Reset all AI cars
        for (int i = 0; i < raceCars.Count; i++)
        {
            var car = raceCars[i];
            car.currentLap = 0;
            car.lapTime = 0f;
            car.raceFinished = false;
            car.racePosition = 1;
            car.enabled = false;
            
            // Reset position with -90 degree rotation
            if (i < startingPositions.Length)
            {
                Quaternion resetRotation = startingPositions[i].rotation * Quaternion.Euler(0, -90, 0);
                car.transform.position = startingPositions[i].position;
                car.transform.rotation = resetRotation;
                car.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
        
        // Reset player car position
        if (playerCarController != null && startingPositions.Length > 0)
        {
            Quaternion playerRotation = startingPositions[0].rotation * Quaternion.Euler(0, -90, 0);
            playerCarController.transform.position = startingPositions[0].position;
            playerCarController.transform.rotation = playerRotation;
            playerCarController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            playerCarController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        
        StartCountdown();
    }

    // Public methods for external control
    public List<AICarController> GetRaceCars()
    {
        return raceCars;
    }

    public List<RaceResult> GetFinalResults()
    {
        return finalResults;
    }

    public bool IsRaceStarted()
    {
        return raceStarted;
    }

    public bool IsRaceFinished()
    {
        return raceFinished;
    }

    public float GetRaceTime()
    {
        return raceStarted ? Time.time - raceStartTime : 0f;
    }
}