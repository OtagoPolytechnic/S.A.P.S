using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//Base written by Rohan Anakin
//Edited by Christain Irvine

/// <summary>
/// Crowd NPC that alternates between wandering and joining a nearby crowd point.
/// Reserves a spot in a <see cref="CrowdPointAllocator"/> when heading to a crowd,
/// idles there briefly, then leaves and resumes normal pathing.
/// </summary>
public class Crowd : NPCPather
{
    private const int CHANGE_DIRECTION_MIN = 20;
    private const int CHANGE_DIRECTION_MAX = 10;
    // Between 0 and 1 chance of randomly picking an crowd point or an edge to path to

    /// <summary>
    /// Chance (0–1) to choose a crowd over an edge/exit when changing direction.
    /// </summary>
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
    /// Leaves the current crowd spot, frees the reservation, and resumes wandering.
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

    /// <summary>
    /// On reaching a goal: if it was a crowd spot, face the crowd and idle briefly;
    /// otherwise use base path completion.
    /// </summary>
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

    /// <summary>
    /// Picks a new crowd allocator from the scene and heads towards it.
    /// </summary>
    protected void SetNewRandomCrowd()
    {
        FindCrowd(NPCSpawner.Instance.crowdPoints);
    }

    /// Tries to reserve a standing point in a random crowd and path to it.
    /// If none are available, resumes normal wandering.
    /// </summary>
    /// <param name="crowdPoints">Candidate crowd roots in the scene.</param>
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
    /// Returns a random crowd allocator from the provided list.
    /// </summary>
    /// <param name="crowdPoints">Crowd parent objects.</param>
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

    /// <summary>
    /// Chooses the next behaviour: abandon current crowd attempt (if any), then
    /// either head to a crowd (by <see cref="crowdPickChance"/>), or wander to a new edge/exit.
    /// </summary>
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

    /// <summary>
    /// Panic overrides: drop any crowd reservation and hand off to base panic behaviour.
    /// </summary>
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

    /// <summary>
    /// Contextual chatter based on current intent (heading to crowd, idle, or leaving).
    /// </summary>
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
