using System.Collections;
using UnityEngine;

public class GuardLeader : Leader
{
    public GameObject player; //set by NPCSpawner
    Follower followingGuard;
    float tickRate = 0.1f, timer;
    const float chaseSpeedMult = 2f;
    bool isGoingToPanic, isChasing;

    protected override void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        // onPanic.AddListener(HandlePanic); //this doesnt work since its only listening to its own panic, not every single NPC

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
        followingGuard = Instantiate(spawnable, homeSpawnPoint.position, Quaternion.identity, parent).AddComponent<Follower>();
        followingGuard.GetComponent<Hurtbox>().enabled = false;
        Destroy(followingGuard.GetComponent<NPCDeathHandler>());
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

            isGoingToPanic = false;

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
        isChasing = true;
    }

    void HandlePanic(Transform panicLocation)
    {
        isGoingToPanic = true;

        //immediately go to the panic
        SetNewGoal(panicLocation);
    }

    IEnumerator LookAround()
    {
        //rotate 360deg to left, then 360deg to right

        yield return null;
    }
}
