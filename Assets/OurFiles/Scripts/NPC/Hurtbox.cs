using System;
using UnityEngine;
using UnityEngine.Events;

// base written by Joshii
// edited by: Jenna

/// <summary>
/// Represents a damageable component that takes damage from a <see cref="Hitbox"/>.
/// Requires a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int health = 100;

    [HideInInspector] public UnityEvent<int> onHealthUpdate = new();
    [HideInInspector] public UnityEvent<GameObject> onDie = new();

    private bool isAlive = true;
    public bool IsAlive { get => isAlive; private set => isAlive = value; }

    /// <summary>
    /// Current health value. Invokes <see cref="onHealthUpdate"/> when changed.  
    /// Triggers death when health reaches 0.
    /// </summary>
    public int Health
    {
        get => health; set
        {
            health = value;
            if (health <= 0 && isAlive)
            {
                Die();
                isAlive = false;
            }
            onHealthUpdate?.Invoke(health);
        }
    }

    /// <summary>
    /// Triggers the death event and notifies listeners.
    /// </summary>
    void Die()
    {
        onDie?.Invoke(gameObject);
    }
}
