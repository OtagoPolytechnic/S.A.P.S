using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public enum NPCType
{
    GuardTutorial,
    GuardLeader,
    GuardFollower,
    Leader,
    Follower,
    Crowd,
    Passerby,
    Target,
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
    [SerializeField] GameObject player;

    [SerializeField]
    private List<NPCType> spawnableTypes;

    [SerializeField]
    private NavMeshSurface genericNavMesh;

    [SerializeField]
    private NavMeshSurface[] allNavMeshes;

    private const float SPAWN_OFFSET_HEIGHT = 0.75f;
    private const int MAX_NPC_COUNT = 300;
    private Vector3 DEFAULT_POSITION = new Vector3(0, 0, 0);

    void Start()
    {
        timer = spawnCooldown;
        SpawnTarget();
        FillScene();
    }

    private void Update()
    {
        if (!enableSpawning) { return; } //up to editor control
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (Contract.Instance.Npcs.Count >= MAX_NPC_COUNT)
            {
                return;
            }
            else
            {
                SpawnRandomNPC();
                timer = spawnCooldown;
            }
            //If NPC start spam spawing, there is an error in the NPC path
        }
    }

    int GetNPCBehaviour()
    {
        int roll1 = Random.Range(0, spawnableTypes.Count);
        int roll2 = Random.Range(0, spawnableTypes.Count);
        return Mathf.Max(roll1, roll2);
    }

    /// <summary>
    /// Spawns a random NPC in a random location with a random goal if not otherwise specified.
    /// </summary>
    /// <param name="home">The edge point to return to if needed</param>
    /// <param name="spawnPoint">The position the NPC will spawn at</param>
    /// <param name="goal">The goal that they handle</param>
    private void SpawnRandomNPC(Vector3 home = default, Vector3 spawnPoint = default, Vector3 goal = default)
    {
        if (home == Vector3.zero)
        {
            home = ReturnSpawnPoint();
        }

        if (spawnPoint == Vector3.zero)
        {
            spawnPoint = home;
        }

        if (goal == Vector3.zero)
        {
            goal = ReturnValidGoalPoint(home);
        }

        SpawnNPC(home, spawnPoint, goal);
    }

    /// <summary>
    /// Spawns and gives an NPC at a random spawn point with a random goal.
    /// </summary>
    /// <param name="home">The edge point to return to if needed</param>
    /// <param name="goal">The goal that they handle</param>
    /// <param name="spawnPoint">The position the NPC will spawn at</param>
    private void SpawnNPC(Vector3 home, Vector3 spawnPoint, Vector3 goal)
    {
        int roll = GetNPCBehaviour();

        // if the spawn point is default, use the home pos to spawn. Otherwise us the spawnPoint
        GameObject activeNPC = Instantiate(npc, spawnPoint + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);

        if (roll == spawnableTypes.IndexOf(NPCType.Passerby))
        {
            activeNPC.AddComponent<Passerby>().SetHomeSpawnGoal(home, spawnPoint, goal);
            activeNPC.gameObject.name = "NPC - Passerby";
        }
        else if (roll == spawnableTypes.IndexOf(NPCType.Leader))
        {
            Leader leader = activeNPC.AddComponent<Leader>();
            leader.SetHomeSpawnGoal(home, spawnPoint, goal);
            leader.SpawnFollowers(npc, parent, characterCreator);
            activeNPC.name = "NPC - Leader";
        }
        else if (roll == spawnableTypes.IndexOf(NPCType.GuardLeader))
        {
            GuardLeader guard = activeNPC.AddComponent<GuardLeader>();
            guard.SetHomeSpawnGoal(home, spawnPoint, goal);
            guard.SpawnFollowers(npc, parent, characterCreator);
            guard.player = player;
            activeNPC.name = "NPC - Guard";
        }
        else if (roll == spawnableTypes.IndexOf(NPCType.Crowd))
        {
            Crowd crowd = activeNPC.AddComponent<Crowd>();
            crowd.SetHomeSpawnGoal(home, spawnPoint, goal);
            crowd.FindCrowd(crowdPoints);
            activeNPC.gameObject.name = "NPC - Crowd";
        }

        characterCreator.SpawnNPCModel(activeNPC.transform, spawnableTypes[roll]);
        activeNPC.transform.LookAt(parent);

        Contract.Instance.AddNPC(activeNPC);
    }

    private void SpawnTarget()
    {
        Vector3 home = ReturnSpawnPoint();
        Vector3 goal = ReturnValidGoalPoint(home);

        GameObject target = Instantiate(npc, home + new Vector3(0, SPAWN_OFFSET_HEIGHT, 0), Quaternion.identity, parent);        

        targetNPC = target.AddComponent<Target>();
        targetNPC.SetHomeSpawnGoal(home, home, goal);
        targetNPC.FindCrowd(crowdPoints);
        targetNPC.name = "TargetNPC";

        characterCreator.SpawnTargetModel(target.transform);
        target.transform.LookAt(parent);

        //spawn target at specific spawn points far from player
        //determine type
        //spawn that type with array of spawn points for path to take.
    }

    /// <summary>
    /// Fills the scene with NPCs until it hits the NPC cap in the contract.
    /// </summary>
    private void FillScene()
    {
        // Disables all NavMeshes that aren't the main one for spawning.
        SetNavMeshStates(false, genericNavMesh);

        // Backup check count to protect infinite while loop
        const int MAX_CHECK_COUNT = MAX_NPC_COUNT + 100;
        int checkCount = 0;

        Bounds navMeshBounds = genericNavMesh.navMeshData.sourceBounds;

        while (Contract.Instance.Npcs.Count < MAX_NPC_COUNT && checkCount < MAX_CHECK_COUNT)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(navMeshBounds.min.x, navMeshBounds.max.x),
                Random.Range(navMeshBounds.min.y, navMeshBounds.max.y),
                Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)
            );

            NavMeshHit hit;
            
            if(NavMesh.SamplePosition(randomPosition, out hit, Mathf.Infinity, 1))
            {
                // Spawns an NPC with a random home, at the desired position with a random goal.
                SpawnRandomNPC(default, hit.position);
            }
            else
            {
                SpawnRandomNPC();
            }

            checkCount++;
        }

        if (checkCount == MAX_CHECK_COUNT)
        {
            Debug.LogWarning("Something in NPCSpawner.cs FillScene() is not spawning properly. The while loop is exciting early.");
        }

        // Enables all NavMeshes when complete.
        SetNavMeshStates(true);
    }

    /// <summary>
    /// Enables or disables NavMeshes with an optional exception set to null by default.
    /// </summary>
    /// <param name="isEnabled"></param>
    /// <param name="exception"></param>
    private void SetNavMeshStates(bool isEnabled, NavMeshSurface exception = null)
    {
        foreach (NavMeshSurface surface in allNavMeshes)
        {
            if (surface != exception)
            {
                surface.gameObject.SetActive(isEnabled);
            }
        }
    }

    /// <summary>
    /// Returns a random spawn point and its index.
    /// </summary>
    private Vector3 ReturnSpawnPoint() //this is a tuple but .NET 7 style or something weird. Understood how it works from here https://stackoverflow.com/questions/34798681/method-with-multiple-return-types
    {
        int roll = Random.Range(0, spawnPoints.Count);
        return spawnPoints[roll].transform.position;
    }

    /// <summary>
    /// Returns a valid goal's position that is not the same as the spawn point's position.
    /// </summary>
    /// <param name="spawnPoint"></param>
    public Vector3 ReturnValidGoalPoint(Vector3 spawnPoint)
    {
        while (true)
        {
            int roll = Random.Range(0, spawnPoints.Count);
            if (spawnPoints[roll].transform.position != spawnPoint)
            {
                return spawnPoints[roll].transform.position;
            }
            else
            {
                continue;
            }
        }
        
    }
}
