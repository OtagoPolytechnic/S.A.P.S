using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Hurtbox))]
public class NPCDeathHandler : MonoBehaviour
{
    [SerializeField] private float ragdollTimer = 10f;
    [SerializeField] private Hurtbox hurtbox;

    private void Start()
    {
        hurtbox.onDie.AddListener(OnDie);
    }

    private void OnDie(GameObject npc)
    {
        npc.GetComponent<NavMeshAgent>().enabled = false;
        npc.GetComponent<Hurtbox>().enabled = false;
        npc.transform.Find("SuspicionLevel").gameObject.SetActive(false);
        npc.transform.Find("VisionCone").gameObject.SetActive(false);

        npc.AddComponent<Rigidbody>();
        npc.AddComponent<CapsuleCollider>();

        StartCoroutine(DespawnCooldown(ragdollTimer));
    }

    private IEnumerator DespawnCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        Despawn();
    }

    private void Despawn()
    {
        GetComponent<NPCPather>().DestroySelf();
    }
}
