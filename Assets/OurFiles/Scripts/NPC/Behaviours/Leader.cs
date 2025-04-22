using System.Collections.Generic;
using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// A controller NPC that leads followers around in a group
/// </summary>
public class Leader : Crowd
{
    private List<Follower> followers = new();
    private List<Transform> standingTransforms;
    /// <summary>
    /// Spawns the followers before finding a crowd to path towards.
    /// </summary>
    /// <param name="spawnable">Reference to the NPC in Resources given by the NPCSpawner</param>
    public void SpawnFollowers(GameObject spawnable)
    {
        int amount = Random.Range(2, 6);
        for (int i = 0; i < amount; i++)
        {
            Follower spawnedFollower = Instantiate(spawnable, homeSpawnPoint.position, Quaternion.identity).AddComponent<Follower>();
            followers.Add(spawnedFollower);
            spawnedFollower.GetComponent<Follower>().FollowLeader(gameObject);

        }
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    public override void FindCrowd(List<GameObject> crowdPoints) //there isn't an easy way to make this not dupe code that I could find that wouldn't require rewriting the Crowd script
    {
        bool foundCrowd = false;
        for (int i = 0; i < crowdPoints.Count; i++)
        {
            crowd = RollCrowd(crowdPoints);
            (standingPoint, standingTransforms) = crowd.ReceiveStandingPointsForGroup(followers);
            if (standingPoint != -1) //found valid spot
            {
                foundCrowd = true;
                SetNewGoal(standingTransforms[0]);
                standingTransforms.Remove(standingTransforms[0]);
                isGoingToCrowd = true;
                break;
            }
        }
        if (!foundCrowd)
        {
            print("Didn't find point going somewhere else");
            SetNewGoal(GetNewRandomGoal()); //tells them to leave the scene
        }
    }

    /// <summary>
    /// Sets followers to join the leader in a crowd
    /// </summary>
    private void SetFollowersToCrowd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GoToStandingPoint(standingTransforms[i]);
        }
    }
    /// <summary>
    /// Sets followers to leave the scene just before the leader deletes themself from the scene
    /// </summary>
    private void SetFollowersToEnd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GoToExitScene(goalPoint);
        }
    }

    protected override void LeaveCrowd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].inCrowd = false;
            standingTransforms[i].GetComponent<CrowdPoint>().isTaken = false;
        }
        base.LeaveCrowd();
    }

    protected override void ChangeDirection()
    {
        //inhibits changing of direction this method should be empty
    }

    protected override void CompletePath()
    {
        if (isGoingToCrowd)
        {
            SetFollowersToCrowd();
        }
        else
        {
            SetFollowersToEnd();
        }
        base.CompletePath();
    }
}
