using System.Collections;
using UnityEngine;

public class GuardLeader : Leader
{
    public GameObject player; //set by NPCSpawner
    GuardFollower followingGuard;
    float tickRate = 0.1f, timer;
    const float chaseSpeedMult = 2.5f, panicSpeedMultiplier = 2f, panicEndSizeMultiplier = 5f;
    bool isGoingToPanic, isChasing;

    protected override void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        NPCEventManager.Instance.onPanic.AddListener(HandlePanic); //listens to every panic event that happens

        timer = tickRate;
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
            StartCoroutine("LookAround");
            agent.updateRotation = true;

            //reset values to default
            isGoingToPanic = false;
            agent.speed *= 1 / panicSpeedMultiplier;
            endSize *= 1 / panicEndSizeMultiplier;
            followingGuard.SetMovementSpeed(1 / panicSpeedMultiplier);

            SetNewRandomCrowd();
        }
        else
        {
            SetNewRandomCrowd();
        }
    }

    protected override void Panic()
    {
        agent.speed *= chaseSpeedMult;
        followingGuard.SetMovementSpeed(chaseSpeedMult);
        isChasing = true;
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
    }

    void HandlePanic(GameObject panicNPC)
    {
        if (panicNPC != gameObject) //stop guards listening to their own panics
        {
            isGoingToPanic = true;
            agent.speed *= panicSpeedMultiplier;
            endSize *= panicEndSizeMultiplier;
            followingGuard.SetMovementSpeed(panicSpeedMultiplier);

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
