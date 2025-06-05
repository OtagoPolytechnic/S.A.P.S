using UnityEngine;

public class GuardFollower : Follower
{
    const float triggerRadius = 0.8f;
    GuardLeader guardLeader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        //add trigger for detecting player arrest
        CapsuleCollider trigger = gameObject.AddComponent<CapsuleCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;

        endSize *= 2;
        guardLeader = leader.GetComponent<GuardLeader>();
    }

/// <summary>
/// Set the value of movement speed for the NavMeshAgent
/// </summary>
/// <param name="speed"></param>
    public void SetMovementSpeed(float speed)
    {
        agent.speed = speed;
    }

/// <summary>
/// Set the NavMesh that the NavMeshAgent uses, requires the 
/// </summary>
/// <param name="id"></param>
    public void SetNavMeshAgentType(int id)
    {
        agent.agentTypeID = id;
    }

    protected override void Panic()
    {
        //stop the guard running away
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && guardLeader.IsChasing)
        {
            NPCEventManager.Instance.onPlayerArrested?.Invoke();
        }
    }
}
