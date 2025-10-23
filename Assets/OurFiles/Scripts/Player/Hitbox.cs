using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Applies damage to a <see cref="Hurtbox"/> component when colliding with it.
/// Requires a collider set as a trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collision> OnHit = new UnityEvent<Collision>();

    [SerializeField] private int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Hurtbox hurtbox;
        if (collision.collider.TryGetComponent<Hurtbox>(out hurtbox))
        {
            hurtbox.Health -= damage;
            OnHit?.Invoke(collision);
        }
    }
}
