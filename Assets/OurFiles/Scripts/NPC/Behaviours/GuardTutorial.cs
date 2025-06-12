using System.Collections;
using UnityEngine;

public class GuardTutorial : NPCPather
{
    public Transform opposingPoint, currentPoint;
    const float chaseSpeedMult = 3f, triggerRadius = 0.8f;
    public GameObject player; //set by NPCSpawner
    public bool IsChasing => isChasing;
    float tickRate = 0.1f, timer, originalSpeed;
    bool isChasing;
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
    }

    protected override void Update()
    {
        if (isChasing)
        {
            //update path to player slower than every frame to save performance
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetNewGoal(player.transform);
                timer = tickRate;
            }
        }
        else
        {
            base.Update();
        }
    }

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

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isChasing)
        {
            TutorialSpawner.Instance.GuardArrest?.Invoke(gameObject);//gameObject pass is needed for listeners
        }
    }

    public void SetPoints(Transform spawn, Transform opposing)
    {
        currentPoint = spawn;
        opposingPoint = opposing;
    }
}
