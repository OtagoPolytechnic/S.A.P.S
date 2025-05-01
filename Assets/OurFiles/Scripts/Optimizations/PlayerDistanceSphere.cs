using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Optimizes certain gameObjects like NPCs based on the distance from the player.
/// </summary>
public class PlayerDistanceSphere : MonoBehaviour
{
    private const float PHYSICS_SPHERE_RADIUS = 50f;
    private const string NPC_TAG = "NPC";
    private SphereCollider trigger;

    void Start()
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, PHYSICS_SPHERE_RADIUS); // TODO Add a layer mask
        for (int i = hitColliders.Length - 1; i > -1; i--)
        {
            SetPerformanceState(hitColliders[i], true);
        }

        trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = PHYSICS_SPHERE_RADIUS;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("In " + other.gameObject);
        SetPerformanceState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Out " + other.gameObject);
        SetPerformanceState(other, false);
    }

    private void SetPerformanceState(Collider other, bool state)
    {
        if (other.CompareTag(NPC_TAG))
        {
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = state ? ObstacleAvoidanceType.LowQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance;

            VisionBehaviour visionCone = other.GetComponentInChildren<VisionBehaviour>();
            visionCone.enabled = state;
        }
    }
}
