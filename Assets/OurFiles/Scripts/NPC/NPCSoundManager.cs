using UnityEngine;

/// <summary>
/// A sound manager for each NPC, controls what voice lines should play based on their type.
/// </summary>
public class NPCSoundManager
{
    private float randomSpeakingChance = 10f;
    private float randomSpeakingMaxChance = 100f;
    private AudioSource audioSource;
    private CharacterVoicePackSO voicePack;
    private bool shouldSpeak = true;

    public float RandomSpeakingChance { get => randomSpeakingChance; set => randomSpeakingChance = value; }
    public bool ShouldSpeak { get => shouldSpeak; set => shouldSpeak = value; }

    public bool IsSpeaking { get => audioSource.isPlaying; }

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
    /// Tries to speak as the NPC, stops if NPC shouldn't be speaking.
    /// </summary>
    /// <param name="clips"></param>
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
    /// Checks if the player should randomly be playing a sound.
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayRandomSound()
    {
        return Random.Range(0, randomSpeakingMaxChance) <= randomSpeakingChance;
    }
}
