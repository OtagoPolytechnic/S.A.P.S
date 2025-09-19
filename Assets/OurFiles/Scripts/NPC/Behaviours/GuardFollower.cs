using UnityEngine;

/// <summary>
/// Guard variant of <see cref="Follower"/>:
/// - Immortal (no Hurtbox / death handler)
/// - Adds a trigger to detect player during a chase and raise arrest
/// - Follows its <see cref="GuardLeader"/> and exposes simple movement/nav config
/// </summary>
public class GuardFollower : Follower
{
    const float triggerRadius = 0.8f;
    GuardLeader guardLeader;
    private NPCExpressionController expr;

    /// <summary>
    /// Makes this guard unkillable, adds a trigger for arrests, caches the leader,
    /// and widens the arrival tolerance for tighter formations.
    /// </summary>
    protected override void Start()
    {
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        CapsuleCollider trigger = gameObject.AddComponent<CapsuleCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;

        endSize *= 2;
        guardLeader = leader.GetComponent<GuardLeader>();
        base.Start();

        expr = GetComponent<NPCExpressionController>();
        if (expr != null) expr.SetIsGuard(true);
    }

    /// <summary>Sets the NavMeshAgent movement speed.</summary>
    /// <param name="speed">Units per second for the agent.</param>
    public void SetMovementSpeed(float speed)
    {
        agent.speed = speed;
    }

    /// <summary>Sets the NavMeshAgent type ID (which NavMesh to use).</summary>
    /// <param name="id">Agent type ID from your NavMesh settings.</param>
    public void SetNavMeshAgentType(int id)
    {
        agent.agentTypeID = id;
    }
    
    /// <summary>
    /// Guards don’t flee on panic; broadcast the event but do not run away.
    /// </summary>
    protected override void Panic()
    {
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
        if (expr != null) expr.TriggerChase();
    }

    /// If the player enters this guard’s trigger while the squad is chasing,
    /// raise the global arrest event.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && guardLeader.IsChasing)
        {
            NPCEventManager.Instance.onPlayerArrested?.Invoke();
        }
    }

    /// <summary>
    /// Change which game object this follower paths towards
    /// </summary>
    /// <param name="leader"></param>
    public void SetLeader(GameObject leader)
    {
        FollowLeader(leader, homePoint);
    }

    protected override void CompletePath()
    {
        if (!inCrowd && State != NPCState.Panic)
        {
            State = NPCState.Walk;
        }

        base.CompletePath();
    }
}
