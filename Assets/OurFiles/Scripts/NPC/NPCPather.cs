using UnityEngine;
using UnityEngine.AI;
//Base written by: Rohan Anakin

/// <summary>
/// Generates the path the NPCs take and handles cleanup of NPCs once finished
/// </summary>
public abstract class NPCPather : MonoBehaviour
{
    public enum NPCState
    {
        Walk,
        Idle,
        Panic,
    }

    [SerializeField]
    protected NavMeshAgent agent;
    protected Transform homeSpawnPoint;
    protected Transform goalPoint;
    [SerializeField]
    [Tooltip("Changes the Range at which NPCs detect when they have finished pathing to be deleted")]
    [Range(0.2f, 0.8f)]
    protected float endSize = 0.5f;
    private float distance = 0.0f;
    private const float runningSpeedMult = 2f;
    private NPCState state;
    public NPCState State 
    { 
        get
        {
            return state;
        } 

        set 
        {
            state = value;
            if (state == NPCState.Panic)
            {
                Panic();
            }
        } 
    }
    virtual protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
/// <summary>
/// Recieves and sets the goal position and home position
/// </summary>
/// <param name="goal"></param>
/// <param name="home"></param>
    public void SetGoalAndHome(Transform goal, Transform home)
    {
        SetNewGoal(goal);
        homeSpawnPoint = home;
    }

    virtual protected void Update() //override and ref base for children
    {
        if (State == NPCState.Walk || State == NPCState.Panic)
        {
            CheckDistance();
        }
    }
    /// <summary>
    /// Begins the NPC's path to the goal given
    /// </summary>
    /// <param name="newGoal"></param>
    protected void SetNewGoal(Transform newGoal)
    {
        State = NPCState.Walk;
        goalPoint = newGoal;
        agent.SetDestination(goalPoint.position);
    }

    protected Transform GetNewRandomGoal()
    {
        return NPCSpawner.Instance.ReturnValidGoalPoint(goalPoint);
    }

    protected void CheckDistance()
    {
        if (agent.hasPath) //waits for generation
        {
            distance = 0.0f;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                distance += Mathf.Abs((corners[i] - corners[i + 1]).magnitude);
            }

            if (distance <= endSize)
            {
                CompletePath();
                State = NPCState.Idle;
            }
        }
    }

    virtual protected void CompletePath()
    {
        DestroySelf();
    }

    /// <summary>
    /// This method removes the NPC from the players coherency zone.
    /// </summary>
    public void RemoveCoherency() 
    {
        foreach (GameObject npc in CoherencyBehaviour.Instance.npcs) //checking if its currently in player coherency before destroying
        {
            if (npc == gameObject)
            {
                CoherencyBehaviour.Instance.npcs.Remove(gameObject);
                break;
            }
        }
    }

    public void DestroySelf()
    {
        RemoveCoherency();

        Contract.Instance.Npcs.Remove(GetComponent<Hurtbox>());
        Destroy(gameObject);
    }

    virtual protected void Panic() //if suspicion is 100 do this
    {
        agent.speed *= runningSpeedMult;
        agent.SetDestination(homeSpawnPoint.position);
        //alert other NPCS to panic
    }
}
