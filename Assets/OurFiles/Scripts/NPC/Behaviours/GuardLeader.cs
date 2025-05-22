using System.Collections;
using UnityEngine;

public class GuardLeader : Leader
{
    Follower followingGuard;
    bool isGoingToPanic;

    protected override void Start()
    {
        //make guard unable to be killed
        Destroy(GetComponent<Hurtbox>());
        Destroy(GetComponent<NPCDeathHandler>());

        onPanic.AddListener(HandlePanic);
    }

    //spawns a single guard follower
    public override void SpawnFollowers(GameObject spawnable, Transform parent, CharacterCreator creator)
    {
        followingGuard = Instantiate(spawnable, homeSpawnPoint.position, Quaternion.identity, parent).AddComponent<Follower>();
        Destroy(followingGuard.GetComponent<Hurtbox>());
        Destroy(followingGuard.GetComponent<NPCDeathHandler>());
        followingGuard.FollowLeader(gameObject, homeSpawnPoint);
        creator.SpawnNPCModel(followingGuard.transform);
        Contract.Instance.AddNPC(followingGuard.gameObject);

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
        //chase player
        //make follower chase player
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
