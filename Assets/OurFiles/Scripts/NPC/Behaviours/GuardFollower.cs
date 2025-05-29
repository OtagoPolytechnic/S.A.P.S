public class GuardFollower : Follower
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //make guard unable to be killed
        GetComponent<Hurtbox>().enabled = false;
        Destroy(GetComponent<NPCDeathHandler>());

        endSize *= 2;
    }

    public void SetMovementSpeed(float speed)
    {
        agent.speed = speed;
    }

    protected override void Panic()
    {
        NPCEventManager.Instance.onPanic?.Invoke(gameObject);
    }
}
