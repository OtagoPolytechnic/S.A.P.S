using UnityEngine;

/// <summary>
/// Forces the object to always face the main camera, making it appear as a "billboard".  
/// Optionally, the object can be kept upright by ignoring camera tilt,  
/// so it only rotates around the vertical axis.
/// </summary>
public class Billboard : MonoBehaviour
{
    [SerializeField] bool stayUpright;

    private GameObject objectToLookAt;

    void Start()
    {
        objectToLookAt = PlayerReferences.Instance.MainCamera.gameObject;
    }

    void Update()
    {
        Vector3 lookAtPosition = objectToLookAt.transform.position;
        if (stayUpright) lookAtPosition = new Vector3(lookAtPosition.x, transform.position.y, lookAtPosition.z);
        transform.LookAt(lookAtPosition);
    }
}
