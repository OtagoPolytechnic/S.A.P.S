using System;
using System.Collections.Generic;
using UnityEngine;

// base written by Joshii

/// <summary>
/// Activates the end platform when completed, or ends the game when player has failed.
/// </summary>
[RequireComponent(typeof(SceneLoader))]
public class Contract : MonoBehaviour
{
    public static Contract Instance { get; private set; }

    [Header("NPCs")]
    [SerializeField] private List<Hurtbox> npcs;
    [SerializeField] private int innocentKillLimit = 3;
    [SerializeField] private Hurtbox target;

    [Header("Scenes")]
    [SerializeField] private string loseScene;
    [SerializeField] private string winScene;

    [Space]
    [SerializeField] private StartEndLevelPlatform endPlatform;
    [SerializeField] private float goalTime;

    private int innocentsKilled = 0;
    public int InnocentsKilled
    {
        get => innocentsKilled; set
        {
            innocentsKilled = value;
            if (innocentsKilled > innocentKillLimit)
            {
                LoseGame();
            }
        }
    }
    public int InnocentKillLimit { get => innocentKillLimit; }
    
    private float timeSpent;
    public float TimeSpent { get => timeSpent; }
    public float MinimumCompleteTime { get => goalTime; }

    private SceneLoader sceneLoader;
    private float startTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        sceneLoader = GetComponent<SceneLoader>();
    }

    void Start()
    {
        foreach (Hurtbox npc in npcs)
        {
            npc.onDie += () => InnocentsKilled++;
        }

        target.onDie += endPlatform.EnablePlatform;
        endPlatform.onGameWin += WinGame;

        startTime = Time.time;
    }

    void WinGame()
    {
        timeSpent = Time.time - startTime;
        sceneLoader.LoadScene(winScene);
    }

    void LoseGame()
    {
        Destroy(this);
        sceneLoader.LoadScene(loseScene);
    }

    public void EndContract() => Destroy(this);
}
