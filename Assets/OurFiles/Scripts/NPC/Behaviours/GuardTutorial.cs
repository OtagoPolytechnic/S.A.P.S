using System.Collections;
using UnityEngine;

/// <summary>
/// Tutorial guard that paces between two points until panic is triggered,
/// then chases the player at increased speed and raises an arrest if they collide.
/// Unkillable and tagged as a tutorial guard for vision logic.
/// </summary>
public class GuardTutorial : NPCPather
{
    public Vector3 opposingPoint, currentPoint;
    const float chaseSpeedMult = 3f, triggerRadius = 0.8f;

    /// <summary>Assigned by <c>NPCSpawner</c>; target to pursue when chasing.</summary>
    public GameObject player; //set by NPCSpawner

     /// <summary>True while actively chasing the player.</summary>
    public bool IsChasing => isChasing;
    float tickRate = 0.1f, timer, originalSpeed;
    bool isChasing;
    NPCExpressionController expr;


    protected override void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        //add trigger for detecting player arrest
        CapsuleCollider trigger = gameObject.AddComponent<CapsuleCollider>();
        trigger.isTrigger = true;
        trigger.radius = triggerRadius;

        timer = tickRate;
        originalSpeed = agent.speed;

        GetComponentInChildren<VisionBehaviour>().isTutorialGuard = true;

        SetNewGoal(opposingPoint);

        expr = GetComponent<NPCExpressionController>();
        if (expr != null) expr.SetIsGuard(true);
    }

    /// <summary>
    /// When chasing: periodically re-path to the player to save perf; else use base behaviour.
    /// </summary>
    protected override void Update()
    {
        if (isChasing)
        {
            //update path to player slower than every frame to save performance
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetNewGoal(player.transform.position);
                timer = tickRate;
            }
        }
        else
        {
            base.Update();
        }
    }

    /// <summary>
    /// On reaching a patrol point (and not chasing), swap targets and wait briefly before moving.
    /// </summary>
    protected override void CompletePath()
    {
        if (isChasing) { return; }
        (currentPoint, opposingPoint) = (opposingPoint, currentPoint); //c# style guide for swapping two values with a tuple instead of multiple lines of code https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0180
        StartCoroutine(WaitBeforeWalking());

    }

    IEnumerator WaitBeforeWalking()
    {
        yield return new WaitForSecondsRealtime(3);
        SetNewGoal(opposingPoint);
    }

    protected override void Panic()
    {
        isChasing = true;
        agent.speed = originalSpeed * chaseSpeedMult;
        if (expr != null) expr.TriggerChase();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isChasing)
        {
            TutorialSpawner.Instance.GuardArrest?.Invoke(gameObject);//gameObject pass is needed for listeners
        }
    }

    /// <summary>
    /// Sets the current and opposing patrol points.
    /// </summary>
    /// <param name="spawn">Initial/current point.</param>
    /// <param name="opposing">Opposite patrol endpoint.</param>
    public void SetPoints(Vector3 spawn, Vector3 opposing)
    {
        currentPoint = spawn;
        opposingPoint = opposing;
    }
}
