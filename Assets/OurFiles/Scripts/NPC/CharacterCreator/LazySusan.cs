using UnityEngine;

/// <summary>
/// Rotates the object (like a turntable/lazy susan) at a base speed, 
/// with speed modified depending on how far the object is rotated away 
/// from facing positive Z. The further it looks away, the stronger the curve modifier.
/// </summary>
public class LazySusan : MonoBehaviour
{
    [SerializeField] private float speedBase = 50f;
    [SerializeField] private float speedLookingAwayModifier = 3;
    [SerializeField] private AnimationCurve lookingAwayCurve;

    void FixedUpdate()
    {
        // this wacky calculation computes 0 when looking towards positive Z, 1 when looking towards negative Z
        float lookingAwayAmount = (180 - Mathf.Abs(180 - transform.eulerAngles.y)) / 180f;
        float lookingAwayModifier = (speedLookingAwayModifier - 1) * lookingAwayCurve.Evaluate(lookingAwayAmount);
        float speedModifier = 1 + lookingAwayModifier;
        transform.Rotate(speedBase * speedModifier * Time.deltaTime * Vector3.up);
    }
}
