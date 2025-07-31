using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

// base written by Joshii

/// <summary>
/// Activates the end platform when completed, or ends the game when player has failed.
/// </summary>
public class Contract : Singleton<Contract>
{
    [Header("NPCs")]
    [SerializeField] private int innocentKillLimit = 3;
    private Hurtbox target;

    [Header("Win")]
    [SerializeField] private string winScene;
    [SerializeField] private float goalTime = 8;

    [Header("Lose")]
    [SerializeField] private string loseScene;
    [SerializeField] private float timeLimit = 60;
    [SerializeField] private bool failAfterTimeLimit;

    [Header("Contract Card")]
    [SerializeField] private GameObject inHandContractCard;
    [SerializeField] private Material targetKilledCardMaterial;
    [SerializeField] private ContractCardManager contractCardManager;

    [Space]
    [SerializeField] private StartEndLevelPlatform endPlatform;
    [SerializeField] private Elevator elevator;
    [SerializeField] private HapticImpulsePlayer leftControllerHaptics;

    private int innocentsKilled = 0;
    public int InnocentsKilled
    {
        get => innocentsKilled; set
        {
            innocentsKilled = value;
            if (innocentsKilled > innocentKillLimit)
            {
                LoseGame(State.KILLED_TOO_MANY_NPCS);
            }
        }
    }
    public int InnocentKillLimit { get => innocentKillLimit; }

    private List<Hurtbox> npcs = new();
    public List<Hurtbox> Npcs { get => npcs; set => npcs = value; }
    public float GoalTime { get => goalTime; }
    public float TimeLimit { get => timeLimit; }
    private float timeSpent;
    public float TimeSpent { get => timeSpent; }

    private float timeStarted;

    public enum State
    {
        PLAYING,
        OUT_OF_TIME,
        KILLED_TOO_MANY_NPCS,
        COMPLETED,
        TARGET_ESCAPED,
        ARRESTED,
    }
    private State currentState = State.PLAYING;
    public State CurrentState { get => currentState; }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        StartCoroutine(FindTarget());

        endPlatform.onGameWin += WinGame;

        timeStarted = Time.time;
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            NPCEventManager.Instance.onPlayerArrested.AddListener(HandlePlayerArrested);
        }
    }

    void Update()
    {
        if (currentState != State.PLAYING) return;

        if (failAfterTimeLimit && Time.time - timeStarted > timeLimit)
        {
            LoseGame(State.OUT_OF_TIME);
        }
    }

    IEnumerator FindTarget()
    {
        // Waits a frame for the target to spawn
        do
        {
            yield return null;
            target = GameObject.Find("TargetNPC").GetComponent<Hurtbox>();

        } while (target == null);

        target.onDie.AddListener(HandleTargetKill);
        
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            target.GetComponent<Target>().OnTargetEscape.AddListener(TargetEscape);
        }
    }

    void TargetEscape()
    {
        endPlatform.EnablePlatform();
        LoseGame(State.TARGET_ESCAPED);
    }

    void WinGame()
    {
        if (currentState == State.COMPLETED) return;
        currentState = State.COMPLETED;
        timeSpent = Time.time - timeStarted;
        StartCoroutine(CloseElevatorEnding());
    }

    void LoseGame(State loseCondition)
    {
        if (currentState != State.PLAYING) return;

        currentState = loseCondition;
        SceneLoader.Instance.LoadScene(loseScene);
    }

    public void AddNPC(GameObject npcObject)
    {
        Hurtbox npc = npcObject.GetComponent<Hurtbox>();
        Npcs.Add(npc);
        npc.onDie.AddListener(HandleNPCDeath);
    }

    void HandleNPCDeath(GameObject npcObject)
    {
        InnocentsKilled++;
    }

    //Complete this code when the target is killed, hurtbox parameter is required from the onDie event
    void HandleTargetKill(GameObject targetHurtbox)
    {
        endPlatform.EnablePlatform();

        if (!contractCardManager.IsCardVisible) contractCardManager.ToggleVision();
        contractCardManager.ChangeCardMaterial(targetKilledCardMaterial);

        //vibrate the controller
        //flash near the controller

        // on wrist fax machine????
        // box w/ paper spawn in it
        // printer sound, typewriting end of line "ding"
        // drop paper = destroy, later is sus item players have to manage
        // "reprint" button
    }

    void HandlePlayerArrested()
    {
        LoseGame(State.ARRESTED);
    }
    
    IEnumerator CloseElevatorEnding()
    {
        yield return StartCoroutine(elevator.CloseDoors());
        SceneLoader.Instance.LoadScene(winScene);
    }

    public void EndContract() => Destroy(gameObject);
}
