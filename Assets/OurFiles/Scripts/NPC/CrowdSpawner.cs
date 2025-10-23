using UnityEngine;


//this script is deprecated 



//Base written by: Rohan Anakin
/// <summary>
/// Spawns NPCs in a crowd formation, arranged in a circle around the origin.
/// </summary>
public class CrowdSpawner : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] 
    private GameObject npc;
    private Quaternion rotation;
   
    [Tooltip("The distance between each NPC that spawns together")]
    [SerializeField]
    [Range(45, 60)]
    private float interNPCDistance = 50f; //cant be const or RO if we want to have editor control

    [Tooltip("The chance each NPC has to spawn in the crowd. A minimum of 3 will always spawn for player coherency")]
    [SerializeField]
    [Range(0,1)]
    private float spawnChance = 0.5f;

    /// <summary>
    /// Instantiates a single NPC at an offset position relative to this transform.
    /// </summary>
    private void SpawnNPC()
    {
        Instantiate(npc, transform.position + new Vector3(1, 0, 0), Quaternion.identity, transform); // I added just 1 for the x value but could be a random number to spice up the look of the crowd
    }
    
    /// <summary>
    /// Spawns a group of NPCs arranged in a circle around this object.
    /// </summary>
    /// <param name="size">The number of positions around the circle to check for spawning.</param>
    public void SpawnGroup(int size = 7)
    {
        int npcCount = 0;
        for (int i = 0; i < size; i++)
        {
            rotation = Quaternion.Euler(0, i * interNPCDistance, 0); //generates the circle of NPCs. just an arbitrary value
            transform.rotation = rotation;
            float roll = Random.value;
            if (roll >= spawnChance)
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
