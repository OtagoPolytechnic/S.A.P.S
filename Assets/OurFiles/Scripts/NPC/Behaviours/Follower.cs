using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// Simple NPC that follows a designated leader:
/// periodically repaths to the leader, can be told to stand in a crowd spot,
/// or to exit the scene. Followers speak less often to reduce group chatter.
/// </summary>
public class Follower : NPCPather
{
    protected GameObject leader;
    private const float RANDOM_SPEAK_CHANCE = 0.25f;
    public bool inCrowd;
    private bool leavingScene;
    private float tickRate = 0.1f;
    private float timer;

    /// <summary>
    /// Initializes the follower with a leader and home position; adjusts agent size/speed.
    /// </summary>
    /// <param name="leader">The GameObject to follow.</param>
    /// <param name="homePos">Home point used by base pathing logic.</param>
    public void FollowLeader(GameObject leader, Vector3 homePos) //basically a constructor
    {
        homePoint = homePos;
        timer = tickRate;
        this.leader = leader;
        agent.radius = 0.4f; 
        agent.speed = Random.Range(agent.speed, 1.75f); //arbitrary value
    }

    protected override void Awake()
    {
        base.Awake();

        AudioSource source = GetComponent<AudioSource>();

        if (source)
        {
            soundManager = new NPCSoundManager(source, VoicePack);
            // Quarters the chance for a follower to speak to reduce over the top noise in groups.
            soundManager.RandomSpeakingChance = soundManager.RandomSpeakingChance * RANDOM_SPEAK_CHANCE;
        }
    }

    protected override void Update()
    {
        if (!inCrowd && !leavingScene) 
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                agent.updateRotation = true;
                SetNewGoal(leader.transform.position);
                timer = tickRate;
            }
        }
        base.Update();

    }
    /// <summary>
    /// Sends the follower to an exit/edge point to despawn or leave the area.
    /// </summary>
    /// <param name="point">World position of the exit.</param>
    public void GoToExitScene(Vector3 point)
    {
        leavingScene = true;
        SetNewGoal(point);
    }

    /// <summary>
    /// Sends the follower to a specific crowd standing point.
    /// </summary>
    /// <param name="point">World position of the crowd spot.</param>
    public void GoToStandingPoint(Vector3 point)
    {
        inCrowd = true;
        SetNewGoal(point);
    }

    /// <summary>
    /// On arrival: face the leader if standing in a crowd; otherwise use base completion when leaving.
    /// </summary>
    protected override void CompletePath()
    {
        if (inCrowd)
        {
            agent.updateRotation = false;
            if (leader != null) transform.LookAt(leader.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        if (leavingScene)
        {
            base.CompletePath();
        }
    }

    /// <summary>
    /// Contextual barks: quieter overall; in-crowd vs. pathing lines.
    /// </summary>
    protected override void RandomSpeak()
    {
        base.RandomSpeak();

        if (soundManager.IsSpeaking) return;

        if (State == NPCState.Idle)
        {
            soundManager.Speak(VoicePack.baseInCrowd);
        }
        else
        {
            soundManager.Speak(VoicePack.leaderPathing);
        }
    }
}
