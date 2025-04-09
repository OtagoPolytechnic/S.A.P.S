using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Crowd : NPCPather
{
    private const int CHANGE_DIRECTION_MIN = 20;
    private const int CHANGE_DIRECTION_MAX = 10;
    // Between 0 and 1 chance of randomly picking an crowd point or an edge to path to
    protected float crowdPickChance = 0.4f;

    private Coroutine waitTillDirectionChange;

    protected virtual void Start()
    {
        StartRandomDirectionCooldown();
    }

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
        ResetRandomDirection();
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

    protected void SetNewRandomCrowd()
    {
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    public void FindCrowd(List<GameObject> crowdPoints)
    {
        bool foundCrowd = false;
        List<GameObject> nonValidPoints = new();
        Transform standingTransform;
        int count = 0;

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

            count++;
        } while (!foundCrowd && count < 1000);

        if (count >= 1000)
        {
            Debug.Log("Couldn't find crowd");
        }

        SetNewGoal(standingTransform);  
        isGoingToCrowd = true;
    }

    private CrowdPointAllocator RollCrowd(List<GameObject> crowdPoints)
    {
        int roll = Random.Range(0, crowdPoints.Count);
        return crowdPoints[roll].GetComponent<CrowdPointAllocator>();
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

    protected void ChangeDirection()
    {
        // Go to crowd
        if (Random.value <= crowdPickChance)
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

    protected void ResetRandomDirection()
    {
        StopRandomDirectionChangeCooldown();
        StartRandomDirectionCooldown();
    }
}
