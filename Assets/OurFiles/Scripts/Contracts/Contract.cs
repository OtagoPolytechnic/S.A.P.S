using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

// base written by Joshii
// edited by Jenna

/// <summary>
/// Manages contract flow: tracks target, timer, and kill limits,
/// enables the end platform, and triggers win/lose scenes.
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
    [SerializeField] private ContractCardManager contractCardManager;

    [Header("Controller Vibration")]
    [SerializeField] private HapticImpulsePlayer leftControllerHaptics;
    [SerializeField, Range(0, 1)] float vibrationIntensity = 1; //0-1 how strong the vibration is -- THIS SHOULD BE IN PLAYER SETTINGS LATER 
    [SerializeField, Min(0)] float vibrationDuration = 0.5f; //in seconds
    [SerializeField, Min(0)] float vibrationFrequency = 0; //vibration Hz, 0 = default

    [Space]
    [SerializeField] private StartEndLevelPlatform endPlatform;
    [SerializeField] private Elevator elevator;

    private int innocentsKilled = 0;
    public int InnocentsKilled
    {
        get => innocentsKilled; private set
        {
            innocentsKilled = value;
            if (innocentsKilled > innocentKillLimit)
            {
                LoseGame(GameState.State.KILLED_TOO_MANY_NPCS);
            }
        }
    }

    private List<Hurtbox> npcs = new();
    public List<Hurtbox> Npcs { get => npcs; set => npcs = value; }

    private float timeStarted;


    void Start()
    {
        GameState.Instance.CurrentState = GameState.State.PLAYING;
        GameState.Instance.CurrentContractState = GameState.ContractState.BEGINNING;

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
        if (GameState.Instance.CurrentState != GameState.State.PLAYING) return;

        if (failAfterTimeLimit && Time.time - timeStarted > timeLimit)
        {
            LoseGame(GameState.State.OUT_OF_TIME);
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
        LoseGame(GameState.State.TARGET_ESCAPED);
    }

    void WinGame()
    {
        if (GameState.Instance.CurrentState == GameState.State.COMPLETED) return;

        // Supply GameWon scene with values via GameState
        GameState.Instance.CurrentState = GameState.State.COMPLETED;
        GameState.Instance.TimeSpent = Time.time - timeStarted;
        GameState.Instance.InnocentsKilled = InnocentsKilled;
        GameState.Instance.GoalTime = goalTime;
        GameState.Instance.TimeLimit = timeLimit;
        GameState.Instance.InnocentKillLimit = innocentKillLimit;
        
        StartCoroutine(CloseElevatorEnding());
    }

    /// <summary>Fail with given reason.</summary>
    /// <param name="loseCondition">Reason for failure.</param>
    void LoseGame(GameState.State loseCondition)
    {
        if (GameState.Instance.CurrentState != GameState.State.PLAYING) return;

        GameState.Instance.CurrentState = loseCondition;
        SceneLoader.Instance.LoadScene(loseScene);
    }

    /// <summary>Register NPC for innocent kill tracking.</summary>
    /// <param name="npcObject">NPC with a Hurtbox.</param>
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

        //change card visuals
        if (!contractCardManager)
        {
            Debug.LogWarning("No Contract found, if you are in tutorial this warning is okay");
            return;
        }
        if (!contractCardManager.IsCardVisible) contractCardManager.ToggleVision();
        contractCardManager.SetCardInfoToTargetKilled();
        contractCardManager.ToggleTargetCamera();

        //vibrate the controller
        StartCoroutine(vibrateController());
    }

    IEnumerator vibrateController()
    {
        for (int i = 0; i < 4; i++)
        {
            leftControllerHaptics.SendHapticImpulse(vibrationIntensity, vibrationDuration, vibrationFrequency);
            yield return new WaitForSeconds(vibrationDuration);
        }
    }

    void HandlePlayerArrested()
    {
        LoseGame(GameState.State.ARRESTED);
    }

    IEnumerator CloseElevatorEnding()
    {
        yield return StartCoroutine(elevator.CloseDoors());
        SceneLoader.Instance.LoadScene(winScene);
    }
}
