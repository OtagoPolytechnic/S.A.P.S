using System;
using UnityEngine;

/// <summary>
/// Takes damage from a Hitbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public event Action<int> onHealthUpdate;
    public event Action onDie;

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
        CoherencyBehaviour.Instance.Count--; // this needs to be refactored for ranged attacks or if you throw the controller
        onDie?.Invoke();
        Destroy(gameObject);
    }
}
