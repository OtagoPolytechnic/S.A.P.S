using System.Collections;
using UnityEngine;

public class GuardLeader : Leader
{
    public GameObject player; //set by NPCSpawner
    GuardFollower followingGuard;
    float tickRate = 0.1f, timer, originalEndSize, originalSpeed;
    const float chaseSpeedMult = 5f, panicSpeedMultiplier = 3f, panicEndSizeMultiplier = 5f, triggerRadius = 0.8f;
    const int navmeshAgentTypeId = -334000983;
    bool isGoingToPanic, isChasing;
    Transform oldGoal;

    protected override void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        //add trigger for detecting player arrest
        CapsuleCollider trigger = gameObject.AddComponent<CapsuleCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;

        NPCEventManager.Instance.onPanic.AddListener(HandlePanic); //listens to every panic event that happens

        timer = tickRate;
        originalEndSize = endSize;
        originalSpeed = agent.speed;
    }

    protected override void Update()
    {
        if (isChasing)
        {
            //update path to player slower than every frame to save performance
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetNewGoal(player.transform);
                timer = tickRate;
            }
        }
        else
        {
            base.Update();
        }
    }

    //spawns a single guard follower
    public override void SpawnFollowers(GameObject spawnable, Transform parent, CharacterCreator creator)
    {
        followingGuard = Instantiate(spawnable, homeSpawnPoint.position, Quaternion.identity, parent).AddComponent<GuardFollower>();
        followingGuard.FollowLeader(gameObject, homeSpawnPoint);
        creator.SpawnNPCModel(followingGuard.transform);
        Contract.Instance.AddNPC(followingGuard.gameObject);
        followingGuard.gameObject.name = "Guard Follower";

        followers.Add(followingGuard);
    }

    protected override void CompletePath()
    {
        if (isGoingToPanic)
        {
            agent.updateRotation = false;
            StartCoroutine(LookAround());
            agent.updateRotation = true;

            //reset values to default
            isGoingToPanic = false;
            agent.speed = originalSpeed;
            endSize = originalEndSize;
            followingGuard.SetMovementSpeed(originalSpeed);

            if (oldGoal)
            {
                SetNewGoal(oldGoal);
                oldGoal = null;
            }
            else
            {
                SetNewRandomCrowd();
            }
        }
        else
        {
            SetNewRandomCrowd();
        }
    }

    protected override void Panic()
    {
        isChasing = true;
        agent.speed = originalSpeed * chaseSpeedMult;
        followingGuard.SetMovementSpeed(originalSpeed * chaseSpeedMult);

        //set navmeshes to include roads and park
        agent.agentTypeID = navmeshAgentTypeId; 
        followingGuard.SetNavMeshAgentType(navmeshAgentTypeId);

        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isChasing)
        {
            NPCEventManager.Instance.onPlayerArrested?.Invoke();
        }
    }

    // go to other NPC panic location 
    void HandlePanic(GameObject panicNPC)
    {
        if (panicNPC != gameObject && !isChasing) //stop guards listening to their own panics, and stopping chasing the player
        {
            isGoingToPanic = true;
            oldGoal = goalPoint;
            agent.speed = originalSpeed * panicSpeedMultiplier;
            endSize = originalEndSize * panicEndSizeMultiplier;
            followingGuard.SetMovementSpeed(originalEndSize * panicSpeedMultiplier);

            //immediately go to the panic
            SetNewGoal(panicNPC.transform);
        }
    }

    IEnumerator LookAround()
    {
        //rotate 360deg to left, then 360deg to right

        yield return null;
    }
}
