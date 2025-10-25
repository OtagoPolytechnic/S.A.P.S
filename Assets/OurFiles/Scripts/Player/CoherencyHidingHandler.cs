using UnityEngine;

public class CoherencyHidingHandler : MonoBehaviour
{
    const string HIDING_TAG = "Anchor";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HIDING_TAG))
        {
            Debug.Log("Hello");
            CoherencyBehaviour.Instance.SetHiding(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HIDING_TAG))
        {
            Debug.Log("Goodbye");
            CoherencyBehaviour.Instance.SetHiding(false);
        }
    }
}
