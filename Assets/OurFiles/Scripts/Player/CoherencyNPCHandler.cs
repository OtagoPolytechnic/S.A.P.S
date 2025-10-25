using UnityEngine;

public class CoherencyNPCHandler : MonoBehaviour
{
    const string NPC_TAG = "NPC";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(NPC_TAG))
        {
            //dont add Guards to the coherency counter
            if (other.gameObject.GetComponent<GuardLeader>() == null && other.gameObject.GetComponent<GuardFollower>() == null)
            {
                CoherencyBehaviour.Instance.npcs.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(NPC_TAG))
        {
            //dont remove Guards from the coherency counter (since theyre not added)
            if (other.gameObject.GetComponent<GuardLeader>() == null && other.gameObject.GetComponent<GuardFollower>() == null)
            {
                CoherencyBehaviour.Instance.npcs.Remove(other.gameObject);
            }
        }
    }
}
