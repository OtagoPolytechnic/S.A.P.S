using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Leader,
    Crowd,
    Passerby
}
//Base written by: Rohan Anakin
/// <summary>
/// Spawns NPCs and sets the path they use to navigate the scene.
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

    private Target targetNPC;
    public Target Target { get => targetNPC; }
    [SerializeField]
    private CharacterCreator characterCreator; 

    private const float SPAWN_OFFSET_HEIGHT = 0.75f;
    void Start()
    {
        characterCreator = GetComponent<CharacterCreator>();
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
        int roll1 = Random.Range(0, NPCType.GetNames(typeof(NPCType)).Length);
        int roll2 = Random.Range(0, NPCType.GetNames(typeof(NPCType)).Length);
        return Mathf.Max(roll1, roll2);
    }
    /// <summary>
    /// Spawns and gives an NPC at a random spawn point with a random goal.
    /// </summary>
    /// <param name="spawn">The edge point to spawn the NPC at</param>
    /// <param name="goal">The goal that they handle</param>
    private void SpawnNPC(Transform spawn, Transform goal)
    {
        int roll = GetNPCBehaviour();

        GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);
        characterCreator.SpawnNPCModel(activeNPC.transform);
        activeNPC.transform.LookAt(parent);
        if (roll == (int)NPCType.Passerby)
        {
            activeNPC.AddComponent<Passerby>().SetGoalAndHome(goal, spawn);
        }
        else if (roll == (int)NPCType.Leader)
        {
            Leader leader = activeNPC.AddComponent<Leader>();
            leader.SetGoalAndHome(goal, spawn);
            leader.SpawnFollowers(npc, parent, characterCreator);
        }
        else //else assume crowd
        {
            Crowd crowd = activeNPC.AddComponent<Crowd>();
            crowd.SetGoalAndHome(goal, spawn);
            crowd.FindCrowd(crowdPoints);
        }
        Contract.Instance.AddNPC(activeNPC);
    }

    private void SpawnTarget()
    {
        Transform spawn = ReturnSpawnPoint();
        Transform goal = ReturnValidGoalPoint(spawn);

        GameObject target = Instantiate(npc, spawn.position + new Vector3(0, SPAWN_OFFSET_HEIGHT, 0), Quaternion.identity, parent);
        characterCreator.SpawnTargetModel(target.transform);
        target.transform.LookAt(parent);

        targetNPC = target.AddComponent<Target>();
        targetNPC.SetGoalAndHome(goal, spawn);
        targetNPC.FindCrowd(crowdPoints);
        targetNPC.name = "TargetNPC";

        //spawn target at specific spawn points far from player
        //determine type
        //spawn that type with array of spawn points for path to take.
    }

    /// <summary>
    /// Returns a random spawn point and its index.
    /// </summary>
    private Transform ReturnSpawnPoint() //this is a tuple but .NET 7 style or something weird. Understood how it works from here https://stackoverflow.com/questions/34798681/method-with-multiple-return-types
    {
        int roll = Random.Range(0, spawnPoints.Count);
        return spawnPoints[roll].transform;
    }

    /// <summary>
    /// Returns a valid goal's transform that is not the same as the spawn point's transform.
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
