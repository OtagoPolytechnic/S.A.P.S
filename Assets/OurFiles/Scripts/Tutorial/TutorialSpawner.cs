using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


//Base written by: Rohan Anakin
/// <summary>
/// A hard coded class to spawn and set NPCs within the tutorial level
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
        TutorialStateManager.Instance.StartInit();
    }

    /// <summary>
    /// Spawns an NPC and sets its behaviour based off the room type
    /// </summary>
    /// <param name="spawn">The place in which the NPC spawns</param>
    /// <param name="roomType">The room the NPC spawns in</param>
    private void SpawnNPC(Transform spawn, int roomType, int iteration = 0)
    {
        GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);

        //when the NPC chatter is added this may need to be disabled here (MAY!!!)
        if (roomType == 0)
        {
            activeNPC.transform.GetChild(0).gameObject.SetActive(false);//should be the vision cone
            TutorialStateManager.Instance.resetTargets.Add(activeNPC);

            characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.Passerby);
            activeNPC.name = $"ResetTarget{iteration}";
        }
        else if (roomType == 1)
        {
            tutorialNPCRespawner.room4NPCs.Add(activeNPC);
            characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.Passerby);
        }
        else if (roomType == 2)
        {
            GuardTutorial guard = activeNPC.AddComponent<GuardTutorial>();
            guard.SetPoints(spawn, room5OpposingWalkingPoints[0].transform);
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
        target.transform.GetChild(0).gameObject.SetActive(false);
        target.transform.rotation = spawn.rotation;

        //spawn target at specific spawn points far from player
        //determine type
        //spawn that type with array of spawn points for path to take.
    }
}
