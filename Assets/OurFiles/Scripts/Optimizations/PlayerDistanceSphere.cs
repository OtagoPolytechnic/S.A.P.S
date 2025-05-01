using UnityEngine;

/// <summary>
/// Optimizes certain gameObjects like NPCs based on the distance from the player.
/// </summary>
public class PlayerDistanceSphere : MonoBehaviour
{
    private const float PHYSICS_SPHERE_SIZE = 50f;
    private const string NPC_TAG = "NPC";

    void Start()
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(transform.position, PHYSICS_SPHERE_SIZE); // TODO Add a layer mask
    }

    private void OnTriggerEnter(Collider other)
    {
        SetPerformanceState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetPerformanceState(other, false);
    }

    private void SetPerformanceState(Collider other, bool state)
    {
        if (other.CompareTag(NPC_TAG))
        {

        }
    }
}
