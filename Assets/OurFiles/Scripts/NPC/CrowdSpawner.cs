using UnityEngine;
/// <summary>
/// Class <c>CrowdSpawner</c> is used to spawn the NPCs randomly in a crowd.
/// </summary>
public class CrowdSpawner : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] 
    private GameObject npc;
    private Quaternion rotation;

    private void SpawnNPC()
    {
        Instantiate(npc, transform.position + new Vector3(1,0,0), Quaternion.identity, transform); // I added just 1 for the x value but could be a random number to spice up the look of the crowd
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
            rotation = Quaternion.Euler(0,  i * 50, 0); //generates the circle of NPCs. just an arbitrary value
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
