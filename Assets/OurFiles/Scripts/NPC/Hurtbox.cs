using System;
using UnityEngine;
using UnityEngine.Events;

// base written by Joshii
// edited by: Jenna

/// <summary>
/// Takes damage from a Hitbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int health = 100;

    [HideInInspector] public UnityEvent<int> onHealthUpdate = new();
    [HideInInspector] public UnityEvent<GameObject> onDie = new();

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

    void Die()
    {
        onDie?.Invoke(gameObject);
        CoherencyBehaviour.Instance.Count--; // this needs to be refactored for ranged attacks or if you throw the controller
        Destroy(gameObject);
    }
}