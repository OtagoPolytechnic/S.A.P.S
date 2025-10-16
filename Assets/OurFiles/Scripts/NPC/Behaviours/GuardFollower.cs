using UnityEngine;

public class GuardFollower : Follower
{
    const float triggerRadius = 0.8f;
    GuardLeader guardLeader;
    private NPCExpressionController expr;

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

    public void SetMovementSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetNavMeshAgentType(int id)
    {
        agent.agentTypeID = id;
    }

    protected override void Panic()
    {
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
        if (expr != null) expr.TriggerChase();
    }

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
