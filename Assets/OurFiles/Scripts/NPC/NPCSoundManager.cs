using UnityEngine;

/// <summary>
/// Manages NPC voice playback, handling random chatter and context-based voice lines.
/// Each NPC has its own instance to control when and how lines are played.
/// </summary>
public class NPCSoundManager
{
    private float randomSpeakingChance = 10f;
    private float randomSpeakingMaxChance = 100f;
    private AudioSource audioSource;
    private CharacterVoicePackSO voicePack;
    private bool shouldSpeak = true;

    /// <summary>
    /// The percentage chance (0–100) that the NPC will play a random voice line when checked.
    /// </summary>
    public float RandomSpeakingChance { get => randomSpeakingChance; set => randomSpeakingChance = value; }
    public bool ShouldSpeak { get => shouldSpeak; set => shouldSpeak = value; }

    public bool IsSpeaking { get => audioSource.isPlaying; }

    /// <summary>
    /// Creates a new sound manager for an NPC.
    /// </summary>
    /// <param name="audioSource">The audio source used to play lines.</param>
    /// <param name="voicePack">The NPC’s assigned voice pack.</param>
    public NPCSoundManager(AudioSource audioSource, CharacterVoicePackSO voicePack)
    {
        this.audioSource = audioSource;
        this.voicePack = voicePack;
    }

    /// <summary>
    /// Stops what the NPC is saying.
    /// </summary>
    public void StopSpeaking()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Plays a random line from the provided clips if the NPC is allowed to speak.
    /// </summary>
    /// <param name="clips">The set of possible audio clips to choose from.</param>
    public void Speak(AudioClip[] clips)
    {
        if (shouldSpeak)
        {
            PlayRandomVoiceLine(clips);
        }
    }

    /// <summary>
    /// Plays a random voiceline from the provided array of lines.
    /// </summary>
    /// <param name="clips"></param>
    public void ForceSpeak(AudioClip[] clips) => PlayRandomVoiceLine(clips);
    
    /// Chooses and plays a random clip from the given array.
    /// </summary>
    /// <param name="clips">The set of possible audio clips to choose from.</param>
    private void PlayRandomVoiceLine(AudioClip[] clips)
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning("No sound clip found. Array must be empty.");
            return;
        }

        int clipIndex = Random.Range(0, clips.Length);

        if (audioSource != null && audioSource.isActiveAndEnabled && clips[clipIndex] != null)
        {
            audioSource.PlayOneShot(clips[clipIndex]);
        }
    }

    /// <summary>
    /// Rolls a random chance and returns whether the NPC should play a line.
    /// </summary>
    public bool CheckPlayRandomSound()
    {
        return Random.Range(0, randomSpeakingMaxChance) <= randomSpeakingChance;
    }
}
