using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Passerby,
    Crowd,
    Leader
}
//Base written by: Rohan Anakin
/// <summary>
/// Class <c>NPCSpawner</c> is used to spawn NPCs and set the path they use to navigate the scene.
/// </summary>
public class NPCSpawner : Singleton<NPCSpawner>
{
    [SerializeField]
    private List<GameObject> spawnPoints = new();
    public List<GameObject> crowdPoints = new();

    [SerializeField]
    private GameObject npc;
    [SerializeField]
    private Transform parent;

    [SerializeField] 
    private float spawnCooldown;
    private float timer;

    [SerializeField]
    private bool enableSpawning = false;

    private NPCType state;

    void Start()
    {
        timer = spawnCooldown;
        SpawnTarget();
    }

    private void Update()
    {
        if (!enableSpawning) { return; } //up to editor control
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (Contract.Instance.Npcs.Count >= 100)
            {
                return;
            }
            else
            {
                Transform spawn = ReturnSpawnPoint();
                SpawnNPC(spawn, ReturnValidGoalPoint(spawn));
                timer = spawnCooldown;
            }
            //If NPC start spam spawing, there is an error in the NPC path
        }
    }

    int GetNPCBehaviour()
    {
        return Random.Range(0, NPCType.GetNames(typeof(NPCType)).Length);
    }
    /// <summary>
    /// Method <c>SpawnNPC</c> spawns and gives an NPC at a random spawn point with a random goal.
    /// </summary>
    /// <param name="spawn"></param>
    /// <param name="goal"></param>
    private void SpawnNPC(Transform spawn, Transform goal)
    {
        int roll = GetNPCBehaviour();
        if (roll == (int)NPCType.Leader)
        {
            return;
        }
        else
        {
            GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);
            activeNPC.transform.LookAt(parent);
            if (roll == (int)NPCType.Passerby)
            {
                //attach passerby script
            }
            else
            {
                activeNPC.AddComponent<Crowd>().SetGoalAndHome(goal, spawn);
                activeNPC.GetComponent<Crowd>().FindCrowd(crowdPoints);
            }
            Contract.Instance.AddNPC(activeNPC);

        }
    }

    private void SpawnTarget()
    {
        //spawn target at specific spawn points far from player
        //determine type
        //spawn that type with array of spawn points for path to take.
    }

    /// <summary>
    /// Tuple <c>ReturnSpawnPoint</c> returns a random spawn point and its index.
    /// </summary>
    private Transform ReturnSpawnPoint() //this is a tuple but .NET 7 style or something weird. Understood how it works from here https://stackoverflow.com/questions/34798681/method-with-multiple-return-types
    {
        int roll = Random.Range(0, spawnPoints.Count);
        return spawnPoints[roll].transform;
    }

    /// <summary>
    /// Function <c>ReturnValidGoalPoint</c> returns a valid goal's transform that is not the same as the spawn point's transform.
    /// </summary>
    /// <param name="spawnPoint"></param>
    public Transform ReturnValidGoalPoint(Transform spawnPoint)
    {
        while (true)
        {
            int roll = Random.Range(0, spawnPoints.Count);
            if (spawnPoints[roll] != spawnPoint.gameObject)
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
