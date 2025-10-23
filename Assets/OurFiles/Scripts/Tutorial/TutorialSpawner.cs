using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


//Base written by: Rohan Anakin

/// <summary>
/// Hard-coded spawner for the Tutorial scene.
/// - Spawns the target and room-specific NPCs
/// - Wires reset targets for <see cref="TutorialStateManager"/>
/// - Seeds guard tutorial behaviour and patrol points
/// </summary>
public class TutorialSpawner : Singleton<TutorialSpawner>
{
    [SerializeField]
    private List<GameObject> room3SpawnPoints = new();
    [SerializeField]
    private List<GameObject> room4SpawnPoints = new();
    [SerializeField]
    private List<GameObject> room5SpawnPoints = new();
    public List<GameObject> room5OpposingWalkingPoints = new();
    List<GameObject> tempResetNPCs = new();

    [SerializeField]
    private GameObject npc;
    [SerializeField]
    private Transform parent;

    [SerializeField]
    GameObject player;
    private TargetTutorial targetNPC;
    public TargetTutorial Target { get => targetNPC; }
    [SerializeField]
    private CharacterCreator characterCreator;
    [SerializeField]
    private TutorialNPCRespawner tutorialNPCRespawner;
    private const float SPAWN_OFFSET_HEIGHT = 0.75f;
    private const float RAGDOLL_TIME = 2f;
    [HideInInspector] public UnityEvent<GameObject> GuardArrest = new();

    void Start()
    {
        SpawnTarget(room3SpawnPoints[0].transform);
        for (int i = 1; i < room3SpawnPoints.Count; i++)
        {
            Transform spawn = room3SpawnPoints[i].transform;
            SpawnNPC(spawn, 0, i);
        }
        for (int i = 0; i < room4SpawnPoints.Count; i++)
        {
            Transform spawn = room4SpawnPoints[i].transform;
            SpawnNPC(spawn, 1);
        }
        for (int i = 0; i < room5SpawnPoints.Count; i++)
        {
            Transform spawn = room5SpawnPoints[i].transform;
            SpawnNPC(spawn, 2);
        }
        TutorialStateManager.Instance.StartInit(tempResetNPCs);
    }

    /// <summary>
    /// Spawns an NPC and configures behaviour based on <paramref name="roomType"/>.
    /// </summary>
    /// <param name="spawn">Transform to spawn at.</param>
    /// <param name="roomType">0 = room3 reset NPC, 1 = room4 passerby, 2 = room5 guard tutorial.</param>
    /// <param name="iteration">Index for naming room3 reset NPCs.</param>
    private void SpawnNPC(Transform spawn, int roomType, int iteration = 0)
    {
        GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);

        //disable voice lines for ALL NPCs in tutorial
        activeNPC.GetComponent<AudioSource>().enabled = false;

        activeNPC.GetComponent<NPCDeathHandler>().ragdollTimer = RAGDOLL_TIME;

        if (roomType == 0)
        {
            activeNPC.GetComponentInChildren<VisionBehaviour>().gameObject.SetActive(false);
            activeNPC.GetComponentInChildren<Billboard>().gameObject.SetActive(false);
            activeNPC.GetComponent<NavMeshAgent>().enabled = false;

            tempResetNPCs.Add(activeNPC);

            characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.Passerby);
            activeNPC.name = $"ResetTarget{iteration}";
        }
        else if (roomType == 1)
        {
            activeNPC.GetComponent<NavMeshAgent>().enabled = false;

            tutorialNPCRespawner.room4NPCs.Add(activeNPC);
            characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.Passerby);
        }
        else if (roomType == 2)
        {
            GuardTutorial guard = activeNPC.AddComponent<GuardTutorial>();
            guard.SetPoints(spawn.position, room5OpposingWalkingPoints[0].transform.position);
            guard.player = player;
            characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.GuardTutorial);
            room5OpposingWalkingPoints.RemoveAt(0);
        }

        activeNPC.transform.rotation = spawn.rotation;
    }
    /// <summary>
    /// Spawns the target
    /// </summary>
    /// <param name="spawn">The place in which the target spawns</param>
    private void SpawnTarget(Transform spawn)
    {
        GameObject target = Instantiate(npc, spawn.position + new Vector3(0, SPAWN_OFFSET_HEIGHT, 0), Quaternion.identity, parent);
        characterCreator.SpawnTargetModel(target.transform);
        targetNPC = target.AddComponent<TargetTutorial>();
        targetNPC.name = "TargetNPC";
        target.GetComponentInChildren<VisionBehaviour>().gameObject.SetActive(false);
        target.GetComponentInChildren<Billboard>().gameObject.SetActive(false);
        target.GetComponent<NavMeshAgent>().enabled = false;
        target.transform.rotation = spawn.rotation;
        target.GetComponent<NPCDeathHandler>().ragdollTimer = RAGDOLL_TIME;

        //spawn target at specific spawn points far from player
        //determine type
        //spawn that type with array of spawn points for path to take.
    }
}
