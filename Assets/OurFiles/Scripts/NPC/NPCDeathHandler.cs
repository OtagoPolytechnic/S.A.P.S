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
        npc.GetComponent<NPCExpressionController>()?
        .SetExpression(NPCExpressionController.ExpressionType.Death);
        
        npc.tag = "Untagged";
        npc.layer = 0;
        if (scene.name != "Tutorial")//name of scene must not be changed!
        {
            NPCPather pather = npc.GetComponent<NPCPather>();
            pather.RemoveCoherency();
            pather.enabled = false;
            npc.GetComponent<CharacterController>().enabled = false;
            npc.GetComponent<NavMeshAgent>().enabled = false;
            pather.SoundManager.ShouldSpeak = false;
            pather.SaySpecificLine(pather.VoicePack.allDie);
        }
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
        if (scene.name == "Tutorial")
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<NPCPather>().DestroySelf();
        }
    }
}
