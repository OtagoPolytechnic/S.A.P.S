using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Crowd : NPCPather
{
    bool isGoingToCrowd;

    IEnumerator WaitToLeaveCrowd(float time)
    {
        yield return new WaitForSeconds(time);
        isGoingToCrowd = false;
        SetNewGoal(GetNewRandomGoal());
        endSize = 0.5f;
    }

    protected override void CompletePath()
    {
        if (isGoingToCrowd)
        {
            StartCoroutine(WaitToLeaveCrowd(Random.Range(10f, 25f)));
        }
        else
        {
            base.CompletePath();
        }
    }

    public void FindCrowd(List<GameObject> crowdPoints)
    {
        int roll = Random.Range(0, crowdPoints.Count);
        SetNewGoal(crowdPoints[roll].transform);  
        isGoingToCrowd = true;
        endSize = 2;
    }
}
