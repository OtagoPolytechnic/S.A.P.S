using System.Collections;
using UnityEngine;

public class Target : Crowd
{
    protected override void Start()
    {
        crowdPickChance = 0.7f;
        
        base.Start();
    }

    protected override void CompletePath()
    {
        Debug.Log("Ended PAth");
        // Check if its in panic
        if (State == NPCState.Panic)
        {
            // TODO End Game due to escape
            base.CompletePath();
        }
        // else if at edge
        else if (!isGoingToCrowd) 
        {
            ChangeDirection();
        }
    }
}
