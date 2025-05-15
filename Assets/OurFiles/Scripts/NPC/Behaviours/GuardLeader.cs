using UnityEngine;

public class GuardLeader : Leader
{
    Follower followingGuard;

    protected override void Start()
    {
        //make guard unable to be killed
        Destroy(GetComponent<Hurtbox>()); 
        Destroy(GetComponent<NPCDeathHandler>());
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

    protected override void Panic()
    {
        //chase player
        //make follower chase player
    }
}
