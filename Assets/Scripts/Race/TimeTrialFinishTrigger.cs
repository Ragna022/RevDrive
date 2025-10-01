using UnityEngine;

public class TimeTrialFinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<TimeTrialManager>().FinishRace();
        }
    }
}
