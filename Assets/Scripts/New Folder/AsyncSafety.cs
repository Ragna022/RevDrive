using UnityEngine;
using System.Threading.Tasks;

public static class AsyncSafety
{
    // Helper method to check if a MonoBehaviour is still valid
    public static bool IsValid(MonoBehaviour behaviour)
    {
        return behaviour != null && 
               behaviour.gameObject != null && 
               behaviour.gameObject.activeInHierarchy;
    }
    
    // Safe version of Task.Delay that cancels if object is destroyed
    public static async Task SafeDelay(float seconds, MonoBehaviour context)
    {
        float elapsed = 0f;
        while (elapsed < seconds && IsValid(context))
        {
            elapsed += Time.deltaTime;
            await Task.Delay(10);
        }
    }
}