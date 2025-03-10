using UnityEngine;

/// <summary>
/// A menu which is attached to the player, will snap back to the player if gets a certain distance away.
/// </summary>
public class PlayerAttachedMenu : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private const float MAX_DISTANCE = 1.5f;
    private const float Y_OFFSET = -0.5f;

    private void Update()
    {
        Vector3 goalPos = transform.position;   

        float distance = Mathf.Abs(Vector3.Distance(cam.position, transform.position));

        if (distance >= MAX_DISTANCE)
        {
            goalPos = cam.transform.position;
        }

        float yDistance = Mathf.Abs(cam.position.y - transform.position.y);


        goalPos = new Vector3(goalPos.x, cam.position.y + Y_OFFSET, goalPos.z);

        LerpTowards(goalPos);
    }

    private void LerpTowards(Vector3 pos)
    {
        transform.position = Vector3.Lerp(transform.position, pos, 0.06f);
    }
}
