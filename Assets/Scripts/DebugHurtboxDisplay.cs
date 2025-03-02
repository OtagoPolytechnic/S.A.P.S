using UnityEngine;

public class DebugHurtboxDisplay : MonoBehaviour
{
    private Hurtbox hurtbox;

    void Awake()
    {
        hurtbox = GetComponentInParent<Hurtbox>();
        if (hurtbox is null)
        {
            Debug.LogWarning("Could not find Hurtbox component in parent. Display is now disabled.");
            enabled = false;
        }
    }
}
