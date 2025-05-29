using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Optimizes certain gameObjects like NPCs based on the distance from the player.
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
    /// Checks if the NPC is inside of the performance sphere and sets their state appropriately.
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
        }
    }
}
