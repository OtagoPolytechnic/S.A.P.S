using UnityEngine;

public class Follower : NPCPather
{
    GameObject leader;
    public bool inCrowd;
    private bool leavingScene;
    private float tickRate = 0.1f;
    private float timer;
    public void FollowLeader(GameObject g) //basically start
    {
        timer = tickRate;
        leader = g;
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

    public void GoToExitScene(Transform point)
    {
        leavingScene = true;
        SetNewGoal(point);
    }

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
