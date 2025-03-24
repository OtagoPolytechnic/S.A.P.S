using UnityEngine;
using UnityEditor;

public class CrowdDebug : EditorWindow
{
    [MenuItem("Tools/Crowd Debugger")]
    public static void ShowEditorWindow()
    {
        GetWindow<CrowdDebug>("Crowd Debugger");
    }
    public int npcCount = 0;
    private CrowdManager crowdManager;
    public void OnGUI()
    {
        GUILayout.Label("Spawn and manage NPCs", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        npcCount = EditorGUILayout.IntField("NPC Count", npcCount);
        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn NPC"))
        {
            if (FindFirstObjectByType<CrowdManager>() == null)
            {
                Instantiate(Resources.Load("Crowd"), new Vector3(0,-0.25f,0), Quaternion.identity);
            }
            crowdManager = FindFirstObjectByType<CrowdManager>();
            if (npcCount == 0)
            {
                crowdManager.SpawnGroup();
            }
            crowdManager.SpawnGroup(npcCount);
        }
    }
}
