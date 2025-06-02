using System.Collections;
using UnityEngine;

/// <summary>
/// Handles physical behavior of an elevator, such as opening and closing of elevator doors
/// </summary>
public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform doorL;
    [SerializeField] private Transform doorR;
    [SerializeField, Range(0, 2)] private float doorOpenDistance;
    [SerializeField] private AnimationCurve doorAnimationCurve;
    [SerializeField] private float doorAnimationDuration;

    private BoxCollider exitBlocker;

    void Start()
    {
        exitBlocker = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Moves doors to be <c>distance</c> apart from the center (creating a gap of <c>distance</c> * 2).<para/>
    /// Uses <c>doorAnimationCurve</c> and <c>doorAnimationDuration</c> to shape the animation.
    /// </summary>
    private IEnumerator MoveDoorsAnimation(float distance)
    {
        float startDistance = doorL.localPosition.x;
        float animationTime = 0;
        while (animationTime < 1)
        {
            // increment up to 1 over the course of doorAnimationDuration secs
            animationTime = Mathf.Clamp(animationTime + Time.deltaTime / doorAnimationDuration, 0, 1);
            float animationValue = doorAnimationCurve.Evaluate(animationTime);

            doorL.localPosition = new(
                Mathf.Lerp(startDistance, distance, animationValue),
                doorL.localPosition.y,
                doorL.localPosition.z
            );
            doorR.localPosition = new(
                -Mathf.Lerp(startDistance, distance, animationValue),
                doorR.localPosition.y,
                doorR.localPosition.z
            );

            yield return null;
        }
    }

    /// <summary>
    /// Shortcut method to open doors, using <c>doorOpenDistance</c>
    /// </summary>
    public void OpenDoors()
    {
        exitBlocker.enabled = false;
        StartCoroutine(MoveDoorsAnimation(doorOpenDistance));
    }

    /// <summary>
    /// Shortcut coroutine to fully close elevator doors. Use <c>yield return</c> to wait for doors to close
    /// </summary>
    public IEnumerator CloseDoors()
    {
        exitBlocker.enabled = true;
        yield return StartCoroutine(MoveDoorsAnimation(0));
    }
}
