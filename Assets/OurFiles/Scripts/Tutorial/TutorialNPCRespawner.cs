using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Respawns Room 4 NPCs when all are killed and player presses the button.
/// </summary>
public class TutorialNPCRespawner : MonoBehaviour
{
    public List<GameObject> room4NPCs = new();

    [SerializeField]
    private List<Transform> room4SpawnPoints = new();

    [SerializeField]
    private GameObject npc;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private CharacterCreator characterCreator;
    [SerializeField]
    private TextMeshPro buttonLabel;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            RespawnNPCs();
        }
    }

    void RespawnNPCs()
    {
        int count = 0;
        foreach (GameObject npc in room4NPCs)
        {
            if (npc == null)
            {
                count++;
            }
        }
        if (count == 4)
        {
            room4NPCs.Clear();
            foreach (Transform spawn in room4SpawnPoints)
            {
                GameObject activeNPC = Instantiate(npc, spawn.position + new Vector3(0, 0.75f, 0), Quaternion.identity, parent);
                characterCreator.SpawnNPCModel(activeNPC.transform, NPCType.Passerby);
                activeNPC.transform.rotation = spawn.rotation;
                room4NPCs.Add(activeNPC);
            }
        }
        else
        {
            buttonLabel.text = "Kill all NPCs to respawn them";
        }
    }
}
