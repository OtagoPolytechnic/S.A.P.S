using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
using System;

//Base written by: Rohan Anakin

/// <summary>
/// Base pathing brain for NPCs:
/// owns a NavMeshAgent, manages home/spawn/goal points, detects arrival,
/// handles destruction/cleanup, and provides common panic + VO logic.
/// </summary>
[RequireComponent(typeof(AudioSource))]
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
    protected Vector3 homePoint;
    protected Vector3 spawnPoint;
    protected Vector3 goalPoint;
    [SerializeField]
    [Tooltip("Changes the Range at which NPCs detect when they have finished pathing to be deleted")]
    [Range(0.2f, 0.8f)]
    protected float endSize = 0.5f;
    private float distance = 0.0f;
    private const float runningSpeedMult = 2f;
    
    protected NPCSoundManager soundManager;
    private CharacterVoicePackSO voicePack;
    protected VisionBehaviour vision;
    protected Animator animator;

    private NPCState state;

    /// <summary>
    /// Current high-level state; setting to <see cref="NPCState.Panic"/> immediately calls <see cref="Panic"/>.
    /// </summary>
    public NPCState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;

            PlayAnimation(state);

            if (state == NPCState.Panic)
            {
                Panic();
            }
        }
    }

    public CharacterVoicePackSO VoicePack { get => voicePack; set => voicePack = value; }
    public NPCSoundManager SoundManager { get => soundManager; }

    /// <summary>Broadcast when this NPC panics (guards listen).</summary>
    [HideInInspector] public UnityEvent<Transform> onPanic = new();
    
    /// <summary>Ensure agent/audio exist and wire up VO manager.</summary>
    virtual protected void Awake()
    {
        if (GetComponent<NavMeshAgent>().enabled == false)
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponentInChildren<VisionBehaviour>();
        animator = GetComponent<Animator>();
        AudioSource source = GetComponent<AudioSource>();
        if (source)
        {
            soundManager = new NPCSoundManager(source, voicePack);
        }
    }

    virtual protected void Start()
    {
        PlayerDistanceSphere.Instance.CheckPerformanceState(gameObject);
        StartCoroutine(WaitForLineCooldown(0.5f));
    }

    /// <summary>
    /// Sets home/spawn/goal in one shot and begins pathing toward the goal.
    /// </summary>
    /// <param name="home">Home/dismiss point.</param>
    /// <param name="spawn">Where this NPC originated.</param>
    /// <param name="goal">First destination to walk to.</param>
    public void SetHomeSpawnGoal(Vector3 home, Vector3 spawn, Vector3 goal)
    {
        SetNewGoal(goal);
        homePoint = home;
        spawnPoint = spawn;
    }

    /// <summary>Walks or panics: check progress toward the goal.</summary>
    virtual protected void Update() //override and ref base for children
    {
        if (State == NPCState.Walk || State == NPCState.Panic)
        {
            CheckDistance();
        }
    }

    /// <summary>
    /// Begin pathing to a new destination and switch to Walk state.
    /// </summary>
    /// <param name="newGoal">World-space destination.</param>
    protected void SetNewGoal(Vector3 newGoal)
    {
        State = NPCState.Walk;
        goalPoint = newGoal;

        if (agent != null)
        {
            agent.SetDestination(goalPoint);
        }
    }

    /// <summary>
    /// Requests another valid exit/edge goal from the spawner, using the current goal as context.
    /// </summary>
    protected Vector3 GetNewRandomGoal()
    {
        return NPCSpawner.Instance.ReturnValidGoalPoint(goalPoint);
    }

    /// <summary>
    /// Uses current NavMesh path corners to measure remaining path distance.
    /// Calls <see cref="CompletePath"/> once within <see cref="endSize"/>.
    /// </summary>
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
                State = NPCState.Idle;
                CompletePath();
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

    /// <summary>
    /// Panic response: run faster, head for <see cref="homePoint"/>, play panic VO, and notify listeners.
    /// </summary>
    virtual protected void Panic() //if suspicion is 100 do this
    {
        agent.speed *= runningSpeedMult;
        agent.SetDestination(homePoint);
        SaySpecificLine(voicePack.basePanic);

        //alert guards to panic
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
    }

    /// <summary>
    /// Random barks based on state/suspicion (skips if already speaking).
    /// </summary>
    virtual protected void RandomSpeak()
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
    }

    /// <summary>
    /// Interrupts current VO and speaks from a specific clip set (for events like death/panic).
    /// </summary>
    /// <param name="lines">Clip pool to choose from.</param>
    public void SaySpecificLine(AudioClip[] lines)
    {
        soundManager.StopSpeaking();
        soundManager.Speak(lines);
    }

    /// <summary>
    /// Stops what is currently being said and bypasses the "should speak" check, generally used for death.
    /// </summary>
    public void ForceSaySpecificLine(AudioClip[] lines)
    {
        soundManager.StopSpeaking();
        soundManager.ForceSpeak(lines);
    }

    private IEnumerator WaitForLineCooldown(float time)
    {
        yield return new WaitForSeconds(time);

        if (soundManager.CheckPlayRandomSound())
        {
            RandomSpeak();
        }

        StartCoroutine(WaitForLineCooldown(time));
    }

    private void PlayAnimation(NPCState state)
    {
        // Resets all triggers before they are called
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }

        if (animator != null)
        {
            animator.SetTrigger(state.ToString());
        }
    }
}
