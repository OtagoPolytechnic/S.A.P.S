using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base written by Joshii

/// <summary>
/// Activates the end platform when completed, or ends the game when player has failed.
/// </summary>
[RequireComponent(typeof(SceneLoader))]
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

    [Space]
    [SerializeField] private StartEndLevelPlatform endPlatform;

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

    private SceneLoader sceneLoader;
    private float timeStarted;

    public enum State
    {
        PLAYING,
        OUT_OF_TIME,
        KILLED_TOO_MANY_NPCS,
        COMPLETED,
        TARGET_ESCAPED,
    }
    private State currentState = State.PLAYING;
    public State CurrentState { get => currentState; }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        sceneLoader = GetComponent<SceneLoader>();

        StartCoroutine(FindTarget());
        
        endPlatform.onGameWin += WinGame;

        timeStarted = Time.time;
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

        target.onDie.AddListener(obj => endPlatform.EnablePlatform());
        target.GetComponent<Target>().OnTargetEscape.AddListener(TargetEscape);
    }

    void TargetEscape()
    {
        endPlatform.EnablePlatform();
        LoseGame(State.TARGET_ESCAPED);
    }

    void WinGame()
    {
        currentState = State.COMPLETED;
        timeSpent = Time.time - timeStarted;
        sceneLoader.LoadScene(winScene);
    }

    void LoseGame(State loseCondition)
    {
        currentState = loseCondition;
        sceneLoader.LoadScene(loseScene);
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

    public void EndContract() => Destroy(gameObject);
}
