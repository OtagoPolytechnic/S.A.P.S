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

    [Header("Other NPCs")]

    [Header("All NPCs")]
    [Tooltip("Lines for when the NPC dies e.g \"Bleaughh\"")]
    public AudioClip[] npcDie;
}
