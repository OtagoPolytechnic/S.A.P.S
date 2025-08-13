using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

// base written by Joshii
// edited by Jenna

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
        get => innocentsKilled; set
        {
            innocentsKilled = value;
            if (innocentsKilled > innocentKillLimit)
            {
                LoseGame(GameState.State.KILLED_TOO_MANY_NPCS);
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


    void Start()
    {
        GameState.Instance.UpdateState(GameState.State.PLAYING);

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
        GameState.Instance.UpdateState(GameState.State.COMPLETED);
        timeSpent = Time.time - timeStarted;
        StartCoroutine(CloseElevatorEnding());
    }

    void LoseGame(GameState.State loseCondition)
    {
        if (GameState.Instance.CurrentState != GameState.State.PLAYING) return;

        GameState.Instance.UpdateState(loseCondition);
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

        //change card visuals
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

    public void EndContract() => Destroy(gameObject);
}
