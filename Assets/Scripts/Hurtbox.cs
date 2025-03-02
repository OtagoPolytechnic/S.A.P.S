using UnityEngine;

/// <summary>
/// Takes damage from a Hitbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Collider trigger;

    public int Health
    {
        get => health; set
        {
            health = value;
        }
    }

    void Awake()
    {
        if (trigger is null)
        {
            Debug.LogWarning("No trigger collider found on hurtbox. Hurtbox disabled.");
            enabled = false;
        }
    }
}
