using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
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

    private void SpawnParticle(Collision other)
    {
        ContactPoint contact = other.GetContact(0);

        GameObject particles = Instantiate(particlePrefab, contact.point, Quaternion.identity);

        StartCoroutine(DeleteAfterTime(particles));
    }

    private IEnumerator DeleteAfterTime(GameObject objectToDelete)
    {
        yield return new WaitForSeconds(particleDeleteTime);
        Destroy(objectToDelete);
    }
}
