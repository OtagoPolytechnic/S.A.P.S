using System.Collections;
using UnityEngine;

public class Passerby : NPCPather
{
    private const float CHANGE_DIRECTION_CHANCE = 0.5f;
    private const int CHANGE_DIRECTION_MIN = 5;
    private const int CHANGE_DIRECTION_MAX = 10;


    private void Start()
    {
        State = NPCState.Walk;

        TryRandomlyChangeDirection();
    }
    
    private void TryRandomlyChangeDirection()
    {
        if (Random.value <= CHANGE_DIRECTION_CHANCE)
        {
            StartCoroutine(WaitChangeDirection(Random.Range((float)CHANGE_DIRECTION_MIN, CHANGE_DIRECTION_MAX)));
        }
    }

    private IEnumerator WaitChangeDirection(float time)
    {
        yield return new WaitForSeconds(time);
        SetNewGoal(GetNewRandomGoal());

        TryRandomlyChangeDirection();
    }
}
