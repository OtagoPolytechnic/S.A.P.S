using System.Collections;
using UnityEngine;

/// <summary>
/// Controls elevator behavior, specifically the physical animation of its doors.  
/// Supports smooth opening/closing with animation curves and manages an exit blocker collider  
/// to prevent the player from leaving while doors are closed.
/// </summary>
public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform doorL;
    [SerializeField] private Transform doorR;
    [SerializeField, Range(0, 2)] private float doorOpenDistance;
    [SerializeField] private AnimationCurve doorAnimationCurve;
    [SerializeField] private float doorAnimationDuration;
    [SerializeField] private BoxCollider exitBlocker;

    /// <summary>
    /// Animates both elevator doors to open or close to a specified <paramref name="distance"/>.  
    /// Doors move symmetrically away from the center, creating a total gap of <c>distance * 2</c>.  
    /// Motion is shaped by <see cref="doorAnimationCurve"/> over <see cref="doorAnimationDuration"/>.
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
    /// Opens elevator doors to the configured <see cref="doorOpenDistance"/>  
    /// and disables the <see cref="exitBlocker"/> so the player can leave.
    /// </summary>
    public void OpenDoors()
    {
        exitBlocker.enabled = false;
        StartCoroutine(MoveDoorsAnimation(doorOpenDistance));
    }

    /// <summary>
    /// Closes elevator doors fully and re-enables the <see cref="exitBlocker"/>.  
    /// Use <c>yield return CloseDoors()</c> to wait until the doors are finished closing.
    /// </summary>
    public IEnumerator CloseDoors()
    {
        exitBlocker.enabled = true;
        yield return StartCoroutine(MoveDoorsAnimation(0));
    }
}
