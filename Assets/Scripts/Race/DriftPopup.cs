using UnityEngine;
using UnityEngine.UI;

public class DriftPopup : MonoBehaviour
{
    public Text comboText; // Assign in prefab
    private Transform followTarget;

    public void Attach(Transform target)
    {
        followTarget = target;
    }

    public void UpdatePopup(int value)
    {
        if (comboText != null)
            comboText.text = value + " pts";
    }

    public void EndPopup()
    {
        Destroy(gameObject, 1.2f);
    }

    void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + Vector3.up * 2f;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}
