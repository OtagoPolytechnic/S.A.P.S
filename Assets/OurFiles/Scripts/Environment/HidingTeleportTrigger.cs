using UnityEngine;

public class HidingTeleportTrigger : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            CoherencyBehaviour.Instance.SetHiding(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            CoherencyBehaviour.Instance.SetHiding(false);
        }
    }
}
