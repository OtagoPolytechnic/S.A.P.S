using System.Collections.Generic;
using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// Group controller NPC: spawns and commands a set of <see cref="Follower"/>s,
/// reserves a crowd group spot for the squad, then leads them to stand or leave.
/// </summary>
public class Leader : Crowd
{
    /// <summary>All followers managed by this leader.</summary>
    protected List<Follower> followers = new();
    
     /// <summary>Per-follower standing positions reserved in the chosen crowd.</summary>
    private List<Transform> standingTransforms;

    protected override void Start()
    {
        GetComponent<Hurtbox>().onDie.AddListener((GameObject g) => SetFollowersToRandomExit());
        base.Start();
    }
    /// <summary>
    /// Spawns followers and assigns models, then finds a crowd for the group.
    /// </summary>
    /// <param name="spawnable">NPC prefab provided by the spawner.</param>
    /// <param name="parent">Parent transform for hierarchy organization.</param>
    /// <param name="creator">Character creator used to skin followers.</param>
    public virtual void SpawnFollowers(GameObject spawnable, Transform parent, CharacterCreator creator)
    {
        int amount = Random.Range(2, 6);
        for (int i = 0; i < amount; i++)
        {
            Follower spawnedFollower = Instantiate(spawnable, spawnPoint, Quaternion.identity, parent).AddComponent<Follower>();
            followers.Add(spawnedFollower);
            spawnedFollower.FollowLeader(gameObject, homePoint);
            creator.SpawnNPCModel(spawnedFollower.transform, NPCType.Follower);
            Contract.Instance.AddNPC(spawnedFollower.gameObject);
            spawnedFollower.gameObject.name = "Follower";
        }
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    /// <summary>
    /// Group-aware crowd selection: reserves a block of standing points for all followers.
    /// Falls back to exit/wander if none are available.
    /// </summary>
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
                SetNewGoal(standingTransforms[0].position);
                standingTransforms.Remove(standingTransforms[0]);
                isGoingToCrowd = true;
                break;
            }
        }
        if (!foundCrowd)
        {
            SetNewGoal(GetNewRandomGoal()); //tells them to leave the scene
        }
    }

     /// <summary>Orders each follower to its reserved crowd position.</summary>
    private void SetFollowersToCrowd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GoToStandingPoint(standingTransforms[i].position);
        }
    }

    /// <summary>Orders followers to exit just before the leader leaves.</summary>
    private void SetFollowersToEnd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GoToExitScene(goalPoint);
        }
    }

    /// <summary>
    /// On leader death: spike suspicion so followers disperse, and send each to a random exit.
    /// </summary>
    private void SetFollowersToRandomExit()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GetComponentInChildren<VisionBehaviour>().Suspicion = 100f;
            followers[i].GoToExitScene(GetNewRandomGoal());
        }
    }

    /// <summary>
    /// Clears follower in-crowd flags and frees the group's reserved points, then hands off to base.
    /// </summary>
    protected override void LeaveCrowd()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].inCrowd = false;
            standingTransforms[i].GetComponent<CrowdPoint>().isTaken = false;
        }
        base.LeaveCrowd();
    }

    /// <summary>
    /// Leaders don’t use the base random direction changes (group is directed explicitly).
    /// </summary>
    protected override void ChangeDirection()
    {
        //inhibits changing of direction this method should be empty
    }

    /// <summary>
    /// On arrival: either fan followers into the crowd, or send them to exit, then use base completion.
    /// </summary>
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

    // This gets a little weird when you have multiple layers of inheritance so there is some duplication of code fomr NPCPather.
    protected override void RandomSpeak()
    {
        if (soundManager.IsSpeaking) return;

        if (State == NPCState.Panic)
        {
            soundManager.Speak(VoicePack.basePanic);
        }
        else if (vision.Suspicion > 0)
        {
            soundManager.Speak(VoicePack.baseSuspicion);
        }
        else if (State == NPCState.Idle)
        {
            soundManager.Speak(VoicePack.baseInCrowd);
        }
        else
        {
            soundManager.Speak(VoicePack.leaderPathing);
        }

        base.RandomSpeak();
    }
}
