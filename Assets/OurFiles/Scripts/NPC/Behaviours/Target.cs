using System.Collections;
using UnityEngine;

public class Target : Crowd
{
    private const int CHANGE_DIRECTION_MIN = 20;
    private const int CHANGE_DIRECTION_MAX = 10;
    // Between 0 and 1 chance of randomly picking an crowd point or an edge to path to
    private const float CROWD_PICK_CHANCE = 0.6f;

    private Coroutine waitTillDirectionChange;

    private void Start()
    {
        State = NPCState.Walk;

        StartRandomDirectionCooldown();
    }

    protected override void CompletePath()
    {
        // Check if its in panic
        if (State == NPCState.Panic)
        {
            // End Game due to escape
            base.CompletePath();
        }
        // If destination is a crowd
        else if (isGoingToCrowd)
        {
            StopRandomDirectionChangeCooldown();
            base.CompletePath();
        }
        // else if at edge
        else 
        {
            ChangeDirection();
            State = NPCState.Walk;
        }
    }

    protected void SetNewRandomCrowd()
    {
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    private void StartRandomDirectionCooldown()
    {
        if (waitTillDirectionChange != null)
        {
            StopCoroutine(waitTillDirectionChange);
            waitTillDirectionChange = null;
        }

        waitTillDirectionChange = StartCoroutine(WaitChangeDirection(Random.Range((float)CHANGE_DIRECTION_MIN, CHANGE_DIRECTION_MAX)));
    }

    private void StopRandomDirectionChangeCooldown()
    {
        if (waitTillDirectionChange != null)
        {
            StopCoroutine(waitTillDirectionChange);
            waitTillDirectionChange = null;
        }
    }

    private IEnumerator WaitChangeDirection(float time)
    {
        yield return new WaitForSeconds(time);

        ChangeDirection();
    }

    private void ChangeDirection()
    {
        // Go to crowd
        if (Random.value <= CROWD_PICK_CHANCE)
        {
            SetNewRandomCrowd();
        }
        // Go to edge
        else
        {
            State = NPCState.Walk;
            waitTillDirectionChange = null;
            SetNewGoal(GetNewRandomGoal());
            StartRandomDirectionCooldown();
        }
    }

    protected override void LeaveCrowd()
    {
        base.LeaveCrowd();
        ResetRandomDirection();
    }

    protected void ResetRandomDirection()
    {
        StopRandomDirectionChangeCooldown();
        StartRandomDirectionCooldown();
    }
}
