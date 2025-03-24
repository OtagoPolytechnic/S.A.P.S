using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] 
    private GameObject npc;
    private Quaternion rotation;

    private void SpawnNPC()
    {
        Instantiate(npc, transform.position + new Vector3(1,0,0), Quaternion.identity, transform);
    }
    /// <summary>
    /// Method <c>SpawnGroup</c> spawns a group of NPCs in a circle around the crowd's origin.
    /// </summary>
    /// <param name="size"></param>
    public void SpawnGroup(int size = 7)
    {
        int npcCount = 0;
        for (int i = 0; i < size; i++)
        {
            rotation = Quaternion.Euler(0,  i * 50, 0);
            transform.rotation = rotation;
            float roll = Random.value;
            if (roll >= 0.5f)
            {
                SpawnNPC();
                npcCount++;
            }
            else if (npcCount < i && i >= 4)
            {
                SpawnNPC();
                npcCount++;
            }
        }
    }
}
