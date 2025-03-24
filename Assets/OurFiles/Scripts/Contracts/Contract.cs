using System.Collections.Generic;
using UnityEngine;

// base written by Joshii

/// <summary>
/// Activates the end platform when completed, or ends the game when player has failed
/// </summary>
[RequireComponent(typeof(SceneLoader))]
public class Contract : MonoBehaviour
{
    public static Contract Instance { get; private set; }

    [Header("NPCs")]
    [SerializeField] private List<Hurtbox> npcs;
    [SerializeField] private int innocentKillLimit = 3;
    [SerializeField] private Hurtbox target;

    [Header("Environment")]
    [SerializeField] private StartEndLevelPlatform endPlatform;
    
    private int innocentsKilled = 0;
    private int InnocentsKilled
    {
        get => innocentsKilled; set
        {
            innocentsKilled = value;
            if (innocentsKilled >= innocentKillLimit)
            {
                Debug.Log("Too many innocents have been killed. Game over.");
                sceneLoader.LoadScene("MainMenuScene");
            }
        }
    }

    private SceneLoader sceneLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
            Debug.Log($"{npc.name}: added onDie listener. innocentsKilled is now {innocentsKilled}");
        }
        
        target.onDie += endPlatform.EnablePlatform;
    }
}
