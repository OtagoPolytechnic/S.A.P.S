using UnityEngine;

/// <summary>
/// A sound manager for each NPC, controls what voice lines should play based on their type.
/// </summary>
public class NPCSoundManager
{
    private float randomSpeakingChance = 0.5f;
    private float randomSpeakingMaxChance = 100f;
    private AudioSource audioSource;
    private CharacterVoicePackSO voicePack;

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

    public void Speak(AudioClip[] clips)
    {
        Debug.LogError("Yelley Time");
        PlayRandomVoiceLine(clips);
    }

    private void PlayRandomVoiceLine(AudioClip[] clips)
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning("No sound clip found. Array must be empty.");
            return;
        }

        int clipIndex = Random.Range(0, clips.Length);

        Debug.Log(clips[clipIndex]);

        audioSource.PlayOneShot(clips[clipIndex]);
    }
}
