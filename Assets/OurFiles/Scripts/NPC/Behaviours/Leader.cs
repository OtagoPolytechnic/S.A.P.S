using System.Collections.Generic;
using UnityEngine;

public class Leader : Crowd
{
    private List<GameObject> followers = new();

    protected override void Start()
    {
        SpawnFollowers();
    }

    void SpawnFollowers()
    {
        int amount = Random.Range(2, 6);
        for (int i = 0; i < amount; i++)
        {
            GameObject spawnedFollower = Instantiate((GameObject)Resources.Load("Resources/NPC"), homeSpawnPoint.position, Quaternion.identity, gameObject.transform);
            followers.Add(spawnedFollower);
            spawnedFollower.AddComponent<Follower>();
            spawnedFollower.GetComponent<Follower>().AttachLeader(gameObject);
        }
    }
}
