using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Hurtbox))]
public class NPCDeathHandler : MonoBehaviour
{
    [SerializeField] private float ragdollTimer = 10f;
    [SerializeField] private float randomSpinStrength = 1f;
    [SerializeField] private Hurtbox hurtbox;
    [SerializeField] private PhysicsMaterial physicsMat;
    Scene scene;

    private void Start()
    {
        hurtbox.onDie.AddListener(OnDie);
        scene = SceneManager.GetActiveScene();
    }

    private void OnDie(GameObject npc)
    {
        NPCExpressionController ec = npc.GetComponentInChildren<NPCExpressionController>(true);
        if (ec != null) ec.TriggerDeath();
        
        npc.tag = "Untagged";
        npc.layer = 0;
        if (scene.name != "Tutorial")//name of scene must not be changed!
        {
            NPCPather pather = npc.GetComponent<NPCPather>();
            if (pather != null)
            {
                pather.RemoveCoherency();
                pather.enabled = false;
                CharacterController cc = npc.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                NavMeshAgent nma = npc.GetComponent<NavMeshAgent>();
                if (nma != null) nma.enabled = false;
                if (pather.SoundManager != null) pather.SoundManager.ShouldSpeak = false;
                pather.ForceSaySpecificLine(pather.VoicePack.allDie);
            }
        }
        Hurtbox hb = npc.GetComponent<Hurtbox>();
        if (hb != null) hb.enabled = false;  
        Transform susp = npc.transform.Find("SuspicionLevel");
        if (susp != null) susp.gameObject.SetActive(false);
        Transform cone = npc.transform.Find("VisionCone");
        if (cone != null) cone.gameObject.SetActive(false);

        Rigidbody rb = npc.AddComponent<Rigidbody>();
        CapsuleCollider cc2 = npc.AddComponent<CapsuleCollider>();
        cc2.height = 2;
        cc2.material = physicsMat;
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
        if (scene.name == "Tutorial")
        {
            Destroy(gameObject);
        }
        else
        {
            NPCPather p = GetComponent<NPCPather>();
            if (p != null) p.DestroySelf();
            else Destroy(gameObject);
        }
    }
}
