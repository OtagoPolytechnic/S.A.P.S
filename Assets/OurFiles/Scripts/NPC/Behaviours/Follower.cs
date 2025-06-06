using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// A dummy NPC that follows commands given by a leader
/// </summary>
public class Follower : NPCPather
{
    GameObject leader;
    public bool inCrowd;
    private bool leavingScene;
    private float tickRate = 0.1f;
    private float timer;

    /// <summary>
    /// Attaches the leader to the follower. Acts as a Constructor method but allows the leader to be passed in as <c>leader</c>
    /// </summary>
    /// <param name="leader"></param>
    public void FollowLeader(GameObject leader, Transform homePos) //basically a constructor
    {
        homeSpawnPoint = homePos;
        timer = tickRate;
        this.leader = leader;
        agent.radius = 0.4f; 
        agent.speed = Random.Range(agent.speed, 1.75f); //arbitrary value
    }

    protected override void Update()
    {
        if (!inCrowd && !leavingScene) 
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.updateRotation = true;
                SetNewGoal(leader.transform);
                timer = tickRate;
            }
        }
        base.Update();

    }
    /// <summary>
    /// Tells the follower to exit the scene. Assumes the point given is a edge point where they can despawn appropriately 
    /// </summary>
    /// <param name="point"></param>
    public void GoToExitScene(Transform point)
    {
        leavingScene = true;
        SetNewGoal(point);
    }
    /// <summary>
    /// Tells the follower to stand in a crowd point
    /// </summary>
    /// <param name="point"></param>
    public void GoToStandingPoint(Transform point)
    {
        inCrowd = true;
        SetNewGoal(point);
    }

    protected override void CompletePath()
    {
        if (inCrowd)
        {
            agent.updateRotation = false;
            transform.LookAt(leader.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        
        if (leavingScene)
        {
            base.CompletePath();
        }
    }
}
