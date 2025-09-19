using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Special crowd NPC marked as the mission target.  
/// Chooses crowds more often, can trigger game-over on escape, 
/// and has unique VO lines.
/// </summary>
public class Target : Crowd
{
    /// <summary>Raised when the target panics and escapes the scene.</summary>
    public UnityEvent OnTargetEscape = new UnityEvent();

    protected override void Start()
    {
        crowdPickChance = 0.7f;

        base.Start();
    }

    protected override void CompletePath()
    {
        // Check if its in panic
        if (State == NPCState.Panic)
        {
            // TODO End Game due to escape
            base.CompletePath();

            OnTargetEscape?.Invoke();
        }
        // else if at edge
        else if (!isGoingToCrowd && State != NPCState.Panic)
        {
            ChangeDirection();
        }
        else
        {
            base.CompletePath();
        }
    }

    protected override void RandomSpeak()
    {
        if (soundManager.IsSpeaking) return;

        if (Random.Range(0f, 1f) <= 0.5f)
        {
            soundManager.Speak(VoicePack.targetLines);
        }

        base.RandomSpeak();
    }
}
