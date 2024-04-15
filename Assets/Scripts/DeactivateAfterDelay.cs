using UnityEngine;

public class DeactivateAfterDelay : MonoBehaviour
{
    public float delay = 2f;

    private void OnEnable()
    {
        Invoke("DeactivateObject", delay);
    }

    private void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}