using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Class <c>NPCSpawnerAndPather</c> is used to spawn NPCs and set the path they use to navigate the scene.
/// </summary>
public class NPCSpawnerAndPather : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> spawnPoints = new List<GameObject>();

    [SerializeField]
    private GameObject npc;
    private int spawnCooldown = 2;
    private float timer = 2;

    [SerializeField]
    private bool enableSpawning = false;

    private void Update()
    {
        if (!enableSpawning) { return; } //up to editor control
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            (Transform spawn, int roll) = ReturnSpawnPoint();
            SpawnNPC(spawn, ReturnValidGoalPoint(roll));
            timer = spawnCooldown;
        }
    }


    /// <summary>
    /// Method <c>SpawnNPC</c> spawns and gives an NPC at a random spawn point with a random goal.
    /// </summary>
    /// <param name="spawn"></param>
    /// <param name="goal"></param>
    private void SpawnNPC(Transform spawn, Transform goal)
    {
        GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        activeNPC.GetComponent<NavMeshAgent>().SetDestination(goal.position);
    }

    /// <summary>
    /// Tuple <c>ReturnSpawnPoint</c> returns a random spawn point and its index.
    /// </summary>
    private (Transform, int) ReturnSpawnPoint() //this is a tuple but .NET 7 style or something weird. Understood how it works from here https://stackoverflow.com/questions/34798681/method-with-multiple-return-types
    {
        int roll = Random.Range(0, spawnPoints.Count - 1);
        return (spawnPoints[roll].transform, roll);
    }

    /// <summary>
    /// Function <c>ReturnValidGoalPoint</c> returns a valid goal's transform that is not the same as the spawn point's transform.
    /// </summary>
    /// <param name="spawnIndex"></param>
    private Transform ReturnValidGoalPoint(int spawnIndex)
    {
        while (true)
        {
            int roll = Random.Range(0, spawnPoints.Count - 1);
            if (spawnPoints[roll] != spawnPoints[spawnIndex])
            {
                return spawnPoints[roll].transform;
            }
            else
            {
                continue;
            }
        }
        
    }
}
