using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CrowdDebug : EditorWindow
{
    [MenuItem("Tools/Crowd Debugger")]
    public static void ShowEditorWindow()
    {
        GetWindow<CrowdDebug>("Crowd Debugger");
    }
    public int npcCount = 0;
    private int spawnPoint = 0;
    private CrowdManager crowdManager;
    public void OnGUI()
    {
        CrowdSpawner crowdSpawner = FindFirstObjectByType<CrowdSpawner>();
        GUILayout.Label("Warning: you are able to spawn mulitple copies of crowds on each other which will have unintended side effects", EditorStyles.largeLabel);
        EditorGUILayout.Space();
        GUILayout.Label("Spawn and manage NPCs", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Clear all Active Crowds"))
            {
                GameObject[] crowds = GameObject.FindGameObjectsWithTag("Crowd");
                foreach (GameObject crowd in crowds)
                {
                    Destroy(crowd);
                }
            }
            EditorGUILayout.Space();

            spawnPoint = EditorGUILayout.IntField("Spawn Point", spawnPoint);
            GUILayout.Label("NPC Count will always default to have atleast 3 NPCs", EditorStyles.miniLabel);
            npcCount = EditorGUILayout.IntField("NPC Count", npcCount);
            EditorGUILayout.Space();

            if (GUILayout.Button("Spawn Individual Crowd"))
            {
                crowdSpawner.SpawnIndividualCrowd(spawnPoint, true);
                crowdManager = FindFirstObjectByType<CrowdManager>();
                if (npcCount == 0 || npcCount < 0 || npcCount > 7)
                {
                    crowdManager.SpawnGroup();
                }
                crowdManager.SpawnGroup(npcCount);
            }

            if (GUILayout.Button("Spawn All Crowds"))
            {
                crowdSpawner.SpawnAllCrowds(new List<int>()); //i pass an empty list to exclude no spawn points
            }
        }
        else
        {
            GUILayout.Label("Enter Play Mode to use this tool", EditorStyles.boldLabel); //this is to stop any scary destruction of objects that could cause catastrophy
        }
    }
}
