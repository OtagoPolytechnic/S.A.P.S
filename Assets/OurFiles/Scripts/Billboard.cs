using UnityEngine;

/// <summary>
/// Will always face the main camera. Can be locked to horizontal mode (the object won't lean over)
/// </summary>
public class Billboard : MonoBehaviour
{
    [SerializeField] bool stayUpright;

    private GameObject objectToLookAt;

    void Start()
    {
        objectToLookAt = Camera.main.gameObject;
    }

    void Update()
    {
        Vector3 lookAtPosition = objectToLookAt.transform.position;
        if (stayUpright) lookAtPosition = new Vector3(lookAtPosition.x, transform.position.y, lookAtPosition.z);
        transform.LookAt(lookAtPosition);
    }
}
