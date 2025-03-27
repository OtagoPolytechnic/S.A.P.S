using UnityEngine;
using UnityEngine.AI;

public class NPCPather : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private Transform homeSpawnPoint;
    private Transform goalPoint;
    float distance = 0.0f;

    void Update()
    {
        if (agent.hasPath)
        {
            distance = 0.0f;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                distance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);
            }

            if (agent.path.status == NavMeshPathStatus.PathComplete && distance <= 0.5f)
            {
                Destroy(gameObject);
            }
        }

    }

    public void SetGoalAndHome(Transform goal, Transform home)
    {
        agent.SetDestination(goal.position);
        goalPoint = goal;
        homeSpawnPoint = home;
    }
}
