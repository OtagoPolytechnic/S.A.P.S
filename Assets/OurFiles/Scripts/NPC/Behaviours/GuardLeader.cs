using System.Collections;
using UnityEngine;

public class GuardLeader : Leader
{
    const float chaseSpeedMult = 5f, panicSpeedMultiplier = 3f, panicEndSizeMultiplier = 5f, triggerRadius = 0.8f;
    const int navmeshAgentTypeId = -334000983;

    public GameObject player;
    public bool IsChasing => isChasing;

    GuardFollower followingGuard;
    float tickRate = 0.1f, timer, originalEndSize, originalSpeed;
    bool isGoingToPanic, isChasing;
    Vector3 oldGoal;
    private NPCExpressionController expr;

    protected override void Start()
    {
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        CapsuleCollider trigger = gameObject.AddComponent<CapsuleCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;

        NPCEventManager.Instance.onPanic.AddListener(HandlePanic);

        timer = tickRate;
        originalEndSize = endSize;
        originalSpeed = agent.speed;

        expr = GetComponent<NPCExpressionController>();
        if (expr != null) expr.SetIsGuard(true);
    }

    protected override void Update()
    {
        if (isChasing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetNewGoal(player.transform.position);
                timer = tickRate;
            }
        }
        else
        {
            base.Update();
        }
    }

    public override void SpawnFollowers(GameObject spawnable, Transform parent, CharacterCreator creator)
    {
        followingGuard = Instantiate(spawnable, spawnPoint, Quaternion.identity, parent).AddComponent<GuardFollower>();
        followingGuard.FollowLeader(gameObject, homePoint);
        creator.SpawnNPCModel(followingGuard.transform, NPCType.GuardLeader);
        Contract.Instance.AddNPC(followingGuard.gameObject);
        followingGuard.gameObject.name = "Guard Follower";
        followers.Add(followingGuard);

        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    protected override void CompletePath()
    {
        if (isGoingToPanic)
        {
            agent.updateRotation = false;
            StartCoroutine(LookAround());
            agent.updateRotation = true;
            isGoingToPanic = false;
            agent.speed = originalSpeed;
            endSize = originalEndSize;
            followingGuard.SetMovementSpeed(originalSpeed);
            if (oldGoal != Vector3.zero)
            {
                SetNewGoal(oldGoal);
                oldGoal = Vector3.zero;
            }
            else
            {
                SetNewRandomCrowd();
            }
        }
        else
        {
            base.CompletePath();
        }
    }

    protected override void Panic()
    {
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = originalSpeed * chaseSpeedMult;
            followingGuard.SetMovementSpeed(originalSpeed * chaseSpeedMult);
            followingGuard.SetLeader(player);

            //set navmeshes to include roads and park
            agent.agentTypeID = navmeshAgentTypeId; 
            followingGuard.SetNavMeshAgentType(navmeshAgentTypeId);

            NPCEventManager.Instance.onPanic?.Invoke(gameObject);
            if (expr != null) expr.TriggerChase();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isChasing)
        {
            NPCEventManager.Instance.onPlayerArrested?.Invoke();
        }
    }

    void HandlePanic(GameObject panicNPC)
    {
        if (panicNPC != gameObject && !isChasing)
        {
            isGoingToPanic = true;
            oldGoal = goalPoint;
            agent.speed = originalSpeed * panicSpeedMultiplier;
            endSize = originalEndSize * panicEndSizeMultiplier;
            followingGuard.SetMovementSpeed(originalSpeed * panicSpeedMultiplier);

            //immediately go to the panic
            SetNewGoal(panicNPC.transform.position);
            if (expr != null) expr.TriggerCalm();
        }
    }

    IEnumerator LookAround()
    {
        yield return null;
    }
}
