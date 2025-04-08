using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Crowd : NPCPather
{
    protected bool isGoingToCrowd;
    CrowdPointAllocator crowd;
    int standingPoint;

    IEnumerator WaitToLeaveCrowd(float time)
    {
        yield return new WaitForSeconds(time);
        LeaveCrowd();
    }

    protected virtual void LeaveCrowd()
    {
        isGoingToCrowd = false;
        agent.updateRotation = true;
        SetNewGoal(GetNewRandomGoal());
        crowd.points[standingPoint].GetComponent<CrowdPoint>().isTaken = false;
    }

    protected override void CompletePath()
    {
        if (isGoingToCrowd)
        {
            agent.updateRotation = false;
            transform.LookAt(crowd.gameObject.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            StartCoroutine(WaitToLeaveCrowd(Random.Range(10f, 25f)));
        }
        else
        {
            base.CompletePath();
        }
    }

    public void FindCrowd(List<GameObject> crowdPoints)
    {
        bool foundCrowd = false;
        List<GameObject> nonValidPoints = new();
        Transform standingTransform;

        do
        {
            crowd = RollCrowd(crowdPoints);
            (standingPoint, standingTransform) = crowd.ReceiveStandingPoint();
            if (standingPoint != -1) //found valid spot
            {
                foundCrowd = true;
            }
            else
            {
                nonValidPoints.Add(crowd.gameObject);
                for (int i = 0; i < crowdPoints.Count; i++)
                {
                    if (!nonValidPoints.Contains(crowdPoints[i]))
                    {
                        if (i == crowdPoints.Count - 1)
                        {
                            SetNewGoal(GetNewRandomGoal()); //tells them to leave the scene
                        }
                        break;
                    }
                }
            }

        } while (!foundCrowd);
        
        SetNewGoal(standingTransform);  
        isGoingToCrowd = true;
    }

    private CrowdPointAllocator RollCrowd(List<GameObject> crowdPoints)
    {
        int roll = Random.Range(0, crowdPoints.Count);
        return crowdPoints[roll].GetComponent<CrowdPointAllocator>();
    }
}
