using System.Collections;
using UnityEngine;

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

    public void OpenDoors()
    {
        exitBlocker.enabled = false;
        StartCoroutine(MoveDoorsAnimation(doorOpenDistance));
    }
    
    public void CloseDoors()
    {
        exitBlocker.enabled = true;
        StartCoroutine(MoveDoorsAnimation(0));
    }
}
