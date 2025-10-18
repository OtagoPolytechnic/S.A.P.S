using System.Collections;
using UnityEngine;

/// <summary>
/// Guard squad leader:
/// - Unkillable, adds a trigger for player arrest checks
/// - Spawns one <see cref="GuardFollower"/> and commands it
/// - On panic: both switch to a chase profile (faster speed, different NavMesh)
/// - When another NPC panics: diverts to investigate briefly, then resumes routine
/// </summary>
public class GuardLeader : Leader
{
    const float chaseSpeedMult = 5f, panicSpeedMultiplier = 3f, panicEndSizeMultiplier = 5f, triggerRadius = 0.8f;
    const int navmeshAgentTypeId = -334000983;

    /// <summary>Assigned by <c>NPCSpawner</c>; the player to pursue when chasing.</summary>
    public GameObject player; //set by NPCSpawner

    /// <summary>True while actively chasing the player.</summary>
    public bool IsChasing => isChasing;

    GuardFollower followingGuard;
    float tickRate = 0.1f, timer, originalEndSize, originalSpeed;
    bool isGoingToPanic, isChasing;
    Vector3 oldGoal;
    private NPCExpressionController expr;

    /// <summary>
    /// Makes the leader unkillable, adds an arrest trigger, subscribes to global panic events,
    /// and caches defaults for later restoration.
    /// </summary>
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

    /// <summary>
    /// Spawns and configures one guard follower, gives it a model, registers it with the contract,
    /// and tracks it as part of the squad.
    /// </summary>
    /// <param name="spawnable">NPC prefab.</param>
    /// <param name="parent">Parent transform for hierarchy organization.</param>
    /// <param name="creator">Character creator to skin the guard.</param>
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

    /// <summary>
    /// After reaching a diversion/panic location, briefly “look around”, restore defaults,
    /// then resume the previous goal or pick a new patrol crowd.
    /// </summary>
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

    /// <summary>
    /// Enter chase mode: both leader and follower speed up and switch to a broader NavMesh (roads/park).
    /// Broadcasts a panic so others can react.
    /// </summary>
    protected override void Panic()
    {
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = originalSpeed * chaseSpeedMult;
            followingGuard.SetMovementSpeed(originalSpeed * chaseSpeedMult);
            followingGuard.SetLeader(player);
            followingGuard.State = NPCState.Panic;

            //set navmeshes to include roads and park
            agent.agentTypeID = navmeshAgentTypeId; 
            followingGuard.SetNavMeshAgentType(navmeshAgentTypeId);

            NPCEventManager.Instance.onPanic?.Invoke(gameObject);
            if (expr != null) expr.TriggerChase();
        }
    }

    /// <summary>
    /// Arrest the player if they enter this leader’s trigger during an active chase.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isChasing)
        {
            NPCEventManager.Instance.onPlayerArrested?.Invoke();
        }
    }

    /// <summary>
    /// Respond to other NPCs’ panic calls (not our own, and not while chasing):
    /// following guard panicking makes leader panic, 
    /// other NPCs make guard temporarily speed up, enlarge arrival tolerance, and divert to the panic spot.
    /// </summary>
    void HandlePanic(GameObject panicNPC)
    {
        //if following guard panics, also panic
        if (panicNPC == followingGuard.gameObject && State != NPCState.Panic)
        {
            State = NPCState.Panic;
        }

        //non-follower and non-self NPC panics
        else if (panicNPC != gameObject && !isChasing)
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

    /// <summary>
    /// Placeholder: rotate to scan the area (left then right), yields a frame for now.
    /// </summary>
    IEnumerator LookAround()
    {
        yield return null;
    }
}
