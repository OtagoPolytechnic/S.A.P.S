using UnityEngine;

/// <summary>
/// Handles the detection of coherency for hiding spots and relays that info to CoherencyBehaviour.cs
/// </summary>
public class CoherencyHidingHandler : MonoBehaviour
{
    const string HIDING_TAG = "Anchor";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HIDING_TAG))
        {
            CoherencyBehaviour.Instance.SetHiding(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HIDING_TAG))
        {
            CoherencyBehaviour.Instance.SetHiding(false);
        }
    }
}
