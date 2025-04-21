using UnityEngine;

public class LazySusan : MonoBehaviour
{
    [SerializeField] private float speedBase = 50f;
    [SerializeField] private float speedLookingAwayModifier = 3;

    void Update()
    {
        // this wacky calculation computes 0 when looking towards positive Z, 1 when looking towards negative Z
        float lookingAwayAmount = (180 - Mathf.Abs(180 - transform.eulerAngles.y)) / 180f;
        float lookingAwayModifier = (speedLookingAwayModifier - 1) * lookingAwayAmount;
        float speedModifier = 1 + lookingAwayModifier;
        transform.Rotate(speedBase * speedModifier * Time.deltaTime * Vector3.up);
    }
}
