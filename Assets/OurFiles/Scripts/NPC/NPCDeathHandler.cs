using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Hurtbox))]
public class NPCDeathHandler : MonoBehaviour
{
    [SerializeField] private float ragdollTimer = 10f;
    [SerializeField] private float randomSpinStrength = 1f;
    [SerializeField] private Hurtbox hurtbox;
    [SerializeField] private PhysicsMaterial physicsMat;

    private void Start()
    {
        hurtbox.onDie.AddListener(OnDie);
    }

    private void OnDie(GameObject npc)
    {
        npc.tag = "Untagged";
        npc.layer = 0;

        NPCPather pather = npc.GetComponent<NPCPather>();
        pather.RemoveCoherency();
        pather.SaySpecificLine(pather.VoicePack.allDie);
        pather.SoundManager.ShouldSpeak = false;
        pather.enabled = false;
        npc.GetComponent<CharacterController>().enabled = false;
        npc.GetComponent<NavMeshAgent>().enabled = false;
        npc.GetComponent<Hurtbox>().enabled = false;
        npc.transform.Find("SuspicionLevel").gameObject.SetActive(false);
        npc.transform.Find("VisionCone").gameObject.SetActive(false);

        Rigidbody rb = npc.AddComponent<Rigidbody>();
        CapsuleCollider cc = npc.AddComponent<CapsuleCollider>();
        cc.height = 2;
        cc.material = physicsMat;
        rb.angularVelocity = new Vector3(
            Random.Range(-randomSpinStrength, randomSpinStrength),
            Random.Range(-randomSpinStrength, randomSpinStrength),
            Random.Range(-randomSpinStrength, randomSpinStrength)
        );

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
