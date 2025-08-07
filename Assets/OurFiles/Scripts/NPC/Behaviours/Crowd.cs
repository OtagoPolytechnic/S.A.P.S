using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//Base written by Rohan Anakin
//Edited by Christain Irvine

public class Crowd : NPCPather
{
    private const int CHANGE_DIRECTION_MIN = 20;
    private const int CHANGE_DIRECTION_MAX = 10;
    // Between 0 and 1 chance of randomly picking an crowd point or an edge to path to
    protected float crowdPickChance = 0.4f;
    private Coroutine waitTillDirectionChange;
    protected bool isLeading = false;
    protected bool isGoingToCrowd;
    protected CrowdPointAllocator crowd;
    protected int standingPoint;

    protected override void Start()
    {
        base.Start();

        StartRandomDirectionCooldown();
    }

    IEnumerator WaitToLeaveCrowd(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        LeaveCrowd();
    }

    /// <summary>
    /// Occurs after standing within a crowd and makes the NPC look for an exit to the scene
    /// </summary>
    protected virtual void LeaveCrowd()
    {
        ResetRandomDirection();
        isGoingToCrowd = false;
        agent.updateRotation = true;
        SetNewGoal(GetNewRandomGoal());
        CrowdPoint point = crowd.points[standingPoint].GetComponent<CrowdPoint>();
        point.isTaken = false;
        point.owner = null;
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

    /// <summary>
    /// Attemps to find a crowd on spawn and path to a point within
    /// </summary>
    /// <param name="crowdPoints">Points the npcs can choose from</param>
    public virtual void FindCrowd(List<GameObject> crowdPoints)
    {
        bool foundCrowd = false;
        Transform standingTransform;
        for (int i = 0; i < crowdPoints.Count; i++)
        {
            crowd = RollCrowd(crowdPoints);
            (standingPoint, standingTransform) = crowd.ReceiveStandingPoint(gameObject);
            if (standingPoint != -1) //found valid spot
            {
                foundCrowd = true;
                SetNewGoal(standingTransform.position);  
                isGoingToCrowd = true;
                break;
            }
        }
        if (!foundCrowd)
        {
            //Debug.LogWarning("Didn't find point going somewhere else"); // If needed for testing lack of pathing
            SetNewGoal(GetNewRandomGoal()); //tells them to leave the scene
        }
        
    }
    /// <summary>
    /// Returns a random crowd from the list of crowd points in the scene
    /// </summary>
    /// <param name="crowdPoints"></param>
    protected CrowdPointAllocator RollCrowd(List<GameObject> crowdPoints)
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

    protected virtual void ChangeDirection()
    {
        if (isGoingToCrowd)
        {
            crowd.points[standingPoint].GetComponent<CrowdPoint>().isTaken = false;
            isGoingToCrowd = false;
            agent.updateRotation = true;
            SetNewGoal(GetNewRandomGoal());
        }
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

    protected override void Panic()
    {
        StopRandomDirectionChangeCooldown();
        if (isGoingToCrowd)
        {
            crowd.points[standingPoint].GetComponent<CrowdPoint>().isTaken = false;
            isGoingToCrowd = false;
            agent.updateRotation = true;
        }
        base.Panic();
    }
    
    protected override void RandomSpeak()
    {
        base.RandomSpeak();

        if (soundManager.IsSpeaking) return;

        if (isGoingToCrowd)
        {
            soundManager.Speak(VoicePack.baseToCrowd);
        }
        else if (State == NPCState.Idle)
        {
            soundManager.Speak(VoicePack.baseInCrowd);
        }
        else
        {
            soundManager.Speak(VoicePack.baseLeaveScene);
        }
    }
}
