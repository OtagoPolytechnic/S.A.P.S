using System.Collections;
using UnityEngine;

public class Passerby : NPCPather
{
    private const float CHANGE_DIRECTION_CHANCE = 0.5f;
    private const int CHANGE_DIRECTION_MIN = 5;
    private const int CHANGE_DIRECTION_MAX = 10;
    private const int MAX_CHANGES = 5;

    private int directionChangeCount = 0;

    protected override void Start()
    {
        base.Start();

        State = NPCState.Walk;

        TryRandomlyChangeDirection();
    }
    
    private void TryRandomlyChangeDirection()
    {
        if (Random.value <= CHANGE_DIRECTION_CHANCE && directionChangeCount < MAX_CHANGES)
        {
            StartCoroutine(WaitChangeDirection(Random.Range((float)CHANGE_DIRECTION_MIN, CHANGE_DIRECTION_MAX)));
        }
    }

    private IEnumerator WaitChangeDirection(float time)
    {
        yield return new WaitForSeconds(time);
        directionChangeCount++;
        SetNewGoal(GetNewRandomGoal());

        TryRandomlyChangeDirection();
    }

    protected override void RandomSpeak()
    {
        base.RandomSpeak();

        if (soundManager.IsSpeaking) return;

        soundManager.Speak(VoicePack.baseLeaveScene);
    }
}
