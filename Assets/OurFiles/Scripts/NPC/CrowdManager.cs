using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] 
    private GameObject npc;
    private Quaternion rotation;

    void SpawnNPC()
    {
        Debug.Log("Spawning NPC");
        Instantiate(npc, transform.position + new Vector3(1,0,0), Quaternion.identity, transform);
    }

    public void SpawnGroup(int size = 7)
    {
        int npcCount = 0;
        for (int i = 0; i < size; i++)
        {
            rotation = Quaternion.Euler(0,  i * 50, 0);
            transform.rotation = rotation;
            Debug.Log("Change rotation: " + rotation);
            float roll = Random.value;
            Debug.Log("Roll: " + roll);
            if (roll >= 0.5f)
            {
                Debug.Log("Roll is greater than 0.5");
                SpawnNPC();
                npcCount++;
            }
            else if (npcCount < i && i >= 4)
            {
                Debug.Log("Not enough NPCs have been spawned");
                SpawnNPC();
                npcCount++;
            }
        }
    }
}
