using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/// <summary>
/// Class <c>CrowdDebug</c> is used to test crowd behaviour from the editor.
/// </summary>
public class CrowdDebug : EditorWindow
{
    //I used this as a base
    //https://medium.com/@dnwesdman/custom-editor-windows-in-unity-15a916f58ac4
    
    [MenuItem("Tools/Crowd Debugger")]
    public static void ShowEditorWindow()
    {
        GetWindow<CrowdDebug>("Crowd Debugger");
    }
    public int npcCount = 0;
    private int spawnPoint = 0;
    private CrowdSpawner crowdSpawner;
    public void OnGUI()
    {
        CrowdManager crowdManager = FindFirstObjectByType<CrowdManager>();
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
                crowdManager.SpawnIndividualCrowd(spawnPoint, true);
                crowdSpawner = FindFirstObjectByType<CrowdSpawner>();
                if (npcCount == 0 || npcCount < 0 || npcCount > 7)
                {
                    crowdSpawner.SpawnGroup();
                }
                crowdSpawner.SpawnGroup(npcCount);
            }

            if (GUILayout.Button("Spawn All Crowds"))
            {
                crowdManager.SpawnAllCrowds(new List<int>()); //i pass an empty list to exclude no spawn points
            }

            if (GUILayout.Button("Spawn All Crowds with some Excluded"))
            {
                List<int> e = new() { 3, 8, 1 };
                crowdManager.SpawnAllCrowds(e);
            }
        }
        else
        {
            GUILayout.Label("Enter Play Mode to use this tool", EditorStyles.boldLabel); //this is to stop any scary destruction of objects that could cause catastrophy
        }
    }
}
