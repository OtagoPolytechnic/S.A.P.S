using System.Collections.Generic;
using UnityEngine;

public class Leader : Crowd
{
    private List<Follower> followers = new();
    private List<Transform> standingTransforms;
    public void SpawnFollowers(GameObject spawnable)
    {
        int amount = Random.Range(2, 6);
        for (int i = 0; i < amount; i++)
        {
            print("spawning follower " + i);
            Follower spawnedFollower = Instantiate(spawnable, homeSpawnPoint.position, Quaternion.identity).AddComponent<Follower>();
            followers.Add(spawnedFollower);
            spawnedFollower.GetComponent<Follower>().FollowLeader(gameObject);

        }
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    public override void FindCrowd(List<GameObject> crowdPoints)
    {
        print("Finding crowd with followers");
        bool foundCrowd = false;
        for (int i = 0; i < crowdPoints.Count; i++)
        {
            crowd = RollCrowd(crowdPoints);
            (standingPoint, standingTransforms) = crowd.ReceiveStandingPointsForGroup(followers);
            if (standingPoint != -1) //found valid spot
            {
                print(standingPoint);
                print("Going to crowd ");
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

    private void SetFollowersToCrowd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GoToStandingPoint(standingTransforms[i]);
        }
    }

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
        //inhibits changing of direction
        print("Stopped Direction change");
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
