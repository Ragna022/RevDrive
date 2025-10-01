using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointCreator : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public GameObject waypointPrefab;
    public float waypointSpacing = 10f;
    
    [Header("Visualization")]
    public Color waypointColor = Color.blue;
    public Color pathColor = Color.green;
    public float waypointSize = 1f;
    
    #if UNITY_EDITOR
    [Header("Editor Tools")]
    [Space(10)]
    [Button("Create Waypoint at Camera")]
    public bool createAtCamera;
    
    [Button("Auto-Generate Circular Track")]
    public bool generateCircular;
    
    [Button("Auto-Generate Figure-8 Track")]
    public bool generateFigure8;
    
    [Button("Clear All Waypoints")]
    public bool clearWaypoints;
    #endif

    void OnValidate()
    {
        #if UNITY_EDITOR
        if (createAtCamera)
        {
            createAtCamera = false;
            CreateWaypointAtSceneCamera();
        }
        
        if (generateCircular)
        {
            generateCircular = false;
            GenerateCircularTrack();
        }
        
        if (generateFigure8)
        {
            generateFigure8 = false;
            GenerateFigure8Track();
        }
        
        if (clearWaypoints)
        {
            clearWaypoints = false;
            ClearAllWaypoints();
        }
        #endif
    }

    public void CreateWaypointAtSceneCamera()
    {
        #if UNITY_EDITOR
        Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
        if (sceneCamera != null)
        {
            CreateWaypointAt(sceneCamera.transform.position);
        }
        else
        {
            Debug.LogWarning("No active scene camera found");
        }
        #endif
    }

    public void CreateWaypointAt(Vector3 position)
    {
        GameObject waypointObj;
        
        if (waypointPrefab != null)
        {
            waypointObj = Instantiate(waypointPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            waypointObj = new GameObject($"Waypoint_{waypoints.Count}");
            waypointObj.transform.parent = transform;
            waypointObj.transform.position = position;
            
            // Add a simple visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.parent = waypointObj.transform;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * 0.5f;
            visual.GetComponent<Renderer>().material.color = waypointColor;
            
            // Remove collider as it's just for visualization
            DestroyImmediate(visual.GetComponent<SphereCollider>());
        }
        
        waypoints.Add(waypointObj.transform);
        Debug.Log($"Created waypoint at {position}");
    }

    public void GenerateCircularTrack(float radius = 50f, int numWaypoints = 16)
    {
        ClearAllWaypoints();
        
        Vector3 center = transform.position;
        
        for (int i = 0; i < numWaypoints; i++)
        {
            float angle = (i / (float)numWaypoints) * 2f * Mathf.PI;
            Vector3 position = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            
            CreateWaypointAt(position);
        }
        
        Debug.Log($"Generated circular track with {numWaypoints} waypoints");
    }

    public void GenerateFigure8Track(float radius = 30f, int waypointsPerLoop = 8)
    {
        ClearAllWaypoints();
        
        Vector3 center = transform.position;
        int totalWaypoints = waypointsPerLoop * 2;
        
        for (int i = 0; i < totalWaypoints; i++)
        {
            float t = i / (float)totalWaypoints;
            Vector3 position;
            
            if (i < waypointsPerLoop)
            {
                // First loop
                float angle = (i / (float)waypointsPerLoop) * 2f * Mathf.PI;
                position = center + new Vector3(
                    Mathf.Cos(angle) * radius - radius * 0.5f,
                    0,
                    Mathf.Sin(angle) * radius
                );
            }
            else
            {
                // Second loop
                int j = i - waypointsPerLoop;
                float angle = (j / (float)waypointsPerLoop) * 2f * Mathf.PI;
                position = center + new Vector3(
                    -Mathf.Cos(angle) * radius + radius * 0.5f,
                    0,
                    Mathf.Sin(angle) * radius
                );
            }
            
            CreateWaypointAt(position);
        }
        
        Debug.Log($"Generated figure-8 track with {totalWaypoints} waypoints");
    }

    public void ClearAllWaypoints()
    {
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            if (waypoints[i] != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DestroyImmediate(waypoints[i].gameObject);
                }
                else
                #endif
                {
                    Destroy(waypoints[i].gameObject);
                }
            }
        }
        
        waypoints.Clear();
        Debug.Log("Cleared all waypoints");
    }

    public Transform[] GetWaypointsArray()
    {
        return waypoints.ToArray();
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        // Draw waypoints
        Gizmos.color = waypointColor;
        foreach (var waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawWireSphere(waypoint.position, waypointSize);
            }
        }

        // Draw path
        Gizmos.color = pathColor;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                int nextIndex = (i + 1) % waypoints.Count;
                if (waypoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
                }
            }
        }
    }
}

#if UNITY_EDITOR
// Custom attribute for creating buttons in the inspector
public class ButtonAttribute : PropertyAttribute
{
    public string MethodName { get; }
    
    public ButtonAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
#endif