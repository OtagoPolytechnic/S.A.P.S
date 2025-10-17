using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
/// <summary>
/// Spawns and manages particle effects when a Hitbox collides with another object.
/// </summary>
public class DamageParticles : MonoBehaviour
{
    [SerializeField] private float particleDeleteTime;
    [SerializeField] private GameObject particlePrefab;

    private Hitbox hitbox;

    private void Start()
    {
        hitbox = GetComponent<Hitbox>();
        
        // Collider is always an npc when this is called
        hitbox.OnHit.AddListener(SpawnParticle);
    }

    /// <summary>
    /// Spawns particle effects at the point of collision.
    /// </summary>
    /// <param name="other">The collision data from the Hitbox.</param>
    private void SpawnParticle(Collision other)
    {
        ContactPoint contact = other.GetContact(0);

        GameObject particles = Instantiate(particlePrefab, contact.point, Quaternion.identity);

        StartCoroutine(DeleteAfterTime(particles));
    }

    /// <summary>
    /// Deletes the given object after a delay.
    /// </summary>
    /// <param name="objectToDelete">The particle object to destroy.</param>
    private IEnumerator DeleteAfterTime(GameObject objectToDelete)
    {
        yield return new WaitForSeconds(particleDeleteTime);
        Destroy(objectToDelete);
    }
}
