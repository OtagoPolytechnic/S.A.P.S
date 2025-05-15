using UnityEngine;

public enum NPCSounds
{
    // Target
    TargetSpeech,

    // Guards
    GuardChatter,
    GuardSuspicion,
    GuardChase,

    // Leader/Follower
    LeaderFollowerPathing,

    // Other NPCs
    BaseLeaveScene,
    BaseToCrowd,
    BaseInCrowd,
    BaseSuspicion,
    BasePanic,

    // All
    Die
}

/// <summary>
/// A sound manager for each NPC, controls what voice lines should play based on their type.
/// </summary>
public class NPCSoundManager
{
    private float randomSpeakingChance = 0.5f;
    private float randomSpeakingMaxChance = 100f;
    private AudioSource audioSource;

    public bool IsSpeaking { get => audioSource.isPlaying; }

    public NPCSoundManager(AudioSource audioSource)
    {
        this.audioSource = audioSource;
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

    public void Speak(NPCSounds speakType)
    {
        switch (speakType)
        {
            case NPCSounds.TargetSpeech:

                break;

        }
    }

    public void PlayRandomVoiceLine(AudioClip[] clips)
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning("No sound clip found. Array must be empty.");
            return;
        }

        int clipIndex = Random.Range(0, clips.Length);

        audioSource.PlayOneShot(clips[clipIndex]);
    }
}
