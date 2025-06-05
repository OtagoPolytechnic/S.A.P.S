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

    private bool isAlive = true;
    public bool IsAlive { get => isAlive; private set => isAlive = value; }
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

    void Die()
    {
        print("I am die");
        print(onDie.GetPersistentEventCount());
        onDie?.Invoke(gameObject);
    }
}
