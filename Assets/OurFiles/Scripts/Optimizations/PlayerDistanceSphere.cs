using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Optimizes NPC behaviour based on the player's distance 
/// by enabling/disabling certain components.
/// </summary>
public class PlayerDistanceSphere : Singleton<PlayerDistanceSphere>
{
    [SerializeField] private float PHYSICS_SPHERE_RADIUS = 50f;
    private const string NPC_TAG = "NPC";
    private SphereCollider trigger;

    void Start()
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, PHYSICS_SPHERE_RADIUS); // TODO Add a layer mask
        for (int i = hitColliders.Length - 1; i > -1; i--)
        {
            SetPerformanceState(hitColliders[i].gameObject, true);
        }

        trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = PHYSICS_SPHERE_RADIUS;
    }

    private void OnTriggerEnter(Collider other)
    {
        SetPerformanceState(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetPerformanceState(other.gameObject, false);
    }

    /// <summary>
    /// Enables or disables NPC components depending on whether they are inside the sphere.
    /// </summary>
    public void CheckPerformanceState(GameObject other)
    {
        float distance = Vector3.Distance(transform.position, other.transform.position);

        SetPerformanceState(other, distance < PHYSICS_SPHERE_RADIUS);
    }

    private void SetPerformanceState(GameObject other, bool state)
    {
        if (other.CompareTag(NPC_TAG))
        {
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = state ? ObstacleAvoidanceType.LowQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;

            VisionBehaviour visionCone = other.GetComponentInChildren<VisionBehaviour>();
            visionCone.enabled = state;

            AudioSource audioSource = other.GetComponent<AudioSource>();
            audioSource.enabled = state;

            Animator animator = other.GetComponent<Animator>();
            animator.enabled = state;
            // sets to intended animation when re enabling
            if (state)
            {
                animator.SetTrigger(other.GetComponent<NPCPather>().State.ToString());
            }
        }
    }
}
