using UnityEngine;

// Base written by Christian.

[CreateAssetMenu(fileName = "CharacterVoicePack", menuName = "Scriptable Objects/CharacterVoicePackSO")]
public class CharacterVoicePackSO : ScriptableObject
{
    [Header("Target")]
    [Tooltip("Lines that the target will say periodically e.g \"I feel like blowing up Chicago today!\"")]
    public AudioClip[] targetLines;

    [Header("Guards")]
    [Tooltip("Chatter between or just for a guard e.g \"I'd really love some donuts right now.\"")]
    public AudioClip[] guardChatter;
    [Tooltip("Lines for when a guard is getting suspicious of the player e.g \"Hey you, what're you up to?\"")]
    public AudioClip[] guardSuspicion;
    [Tooltip("Lines for when a guard is chasing the player e.g \"Hey, get back here!\"")]
    public AudioClip[] guardChase;

    [Header("Leader/Follower")]
    [Tooltip("When a Leader or Follower is pathing e.g \"Do you guys want to head to the restaurant?\"")]
    public AudioClip[] leaderPathing;

    [Header("Other NPCs")]
    [Tooltip("When an NPC is pathing to an edge of scene e.g \"So excited to go to the supermarket!\"")]
    public AudioClip[] baseLeaveScene;
    [Tooltip("When an NPC is pathing to a crowd (Not followers/leaders) e.g \"So excited to see my buddies!\"")]
    public AudioClip[] baseToCrowd;
    [Tooltip("When an NPC is sitting in a crowd e.g \"Are you going to the bash tonight? It's going to be kicking!\"")]
    public AudioClip[] baseInCrowd;
    [Tooltip("When an NPC is getting suspicious of the player (Not guard) e.g \"Huh, what an odd fellow.\"")]
    public AudioClip[] baseSuspicion;
    [Tooltip("When an NPC is panicking (Not guard) e.g \"Oh golly! Get out of here!\"")]
    public AudioClip[] basePanic;

    [Header("All NPCs")]
    [Tooltip("Lines for when an NPC dies e.g \"Bleaughh\"")]
    public AudioClip[] allDie;
}
