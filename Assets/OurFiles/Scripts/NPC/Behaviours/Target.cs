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
        Debug.Log(State);

        // Check if its in panic
        if (State == NPCState.Panic)
        {
            // TODO End Game due to escape
            base.CompletePath();

            Debug.Log("YOU LOST YOU LOSER");
        }
        // else if at edge
        else if (!isGoingToCrowd && State != NPCState.Panic) 
        {
            Debug.Log("Changing direction");
            ChangeDirection();
        }
        else
        {
            base.CompletePath();
        }
    }
}
