using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Target : Crowd
{
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
}
