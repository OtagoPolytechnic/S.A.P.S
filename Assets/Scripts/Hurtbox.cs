using System;
using UnityEngine;

/// <summary>
/// Takes damage from a Hitbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Collider trigger;

    public event Action<int> onHealthUpdate;

    public int Health
    {
        get => health; set
        {
            health = value;
            if (health <= 0)
            {
                Die();
            }
            onHealthUpdate?.Invoke(health);
        }
    }

    void Awake()
    {
        if (trigger == null)
        {
            Debug.LogWarning("No trigger collider found on hurtbox. Hurtbox disabled.");
            enabled = false;
        }
    }

    void Die()
    {
        Destroy(transform.parent.gameObject);
    }
}
