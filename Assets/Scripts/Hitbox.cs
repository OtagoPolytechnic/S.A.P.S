using UnityEngine;

/// <summary>
/// Causes damage to a Hurtbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
public class Hitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Collider trigger;

    void Awake()
    {
        if (trigger is null)
        {
            Debug.LogWarning("No trigger collider found on hitbox. Hitbox disabled.");
            enabled = false;
        }
    }
}
