using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class <c>NPCPather</c> generates the path the NPCs take and handles cleanup of NPCs once finished
/// </summary>
public class NPCPather : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private Transform homeSpawnPoint;
    private Transform goalPoint;
    float distance = 0.0f;

    void Update()
    {
        if (agent.hasPath) //waits for generation
        {
            distance = 0.0f;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                distance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);
            }

            if (agent.path.status == NavMeshPathStatus.PathComplete && distance <= 0.5f)
            {
                Destroy(gameObject); //BUG: needs to remove from coherency if it leaves the scene
            }
        }

    }


/// <summary>
/// Method <c>SetGoalAndHome</c> recieves the Goal position and Home position for use from the class <c>NPC Spawner</c>
/// </summary>
/// <param name="goal"></param>
/// <param name="home"></param>
    public void SetGoalAndHome(Transform goal, Transform home)
    {
        agent.SetDestination(goal.position);
        goalPoint = goal;
        homeSpawnPoint = home;
    }
}
