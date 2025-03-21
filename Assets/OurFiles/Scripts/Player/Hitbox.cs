using UnityEngine;

/// <summary>
/// Causes damage to a Hurtbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox;
        if (other.TryGetComponent<Hurtbox>(out hurtbox))
        {
            hurtbox.Health -= damage;
        }
    }
}
