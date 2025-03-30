using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Causes damage to a Hurtbox component when intersecting.
/// Requires a trigger collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collision> OnHit = new UnityEvent<Collision>();

    [SerializeField] private int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision);

        Hurtbox hurtbox;
        if (collision.collider.TryGetComponent<Hurtbox>(out hurtbox))
        {
            hurtbox.Health -= damage;
            OnHit?.Invoke(collision);
        }
    }
}
