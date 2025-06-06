using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


//Base written by: Rohan Anakin
/// <summary>
/// Class <c>CrowdDebug</c> is used to test crowd behaviour from the editor.
/// </summary>
public class CrowdDebug : EditorWindow
{
    //I used this as a base
    //https://medium.com/@dnwesdman/custom-editor-windows-in-unity-15a916f58ac4

    private Material targetOriginalMaterial;
    private Material targetDebugMaterial;

    [MenuItem("Tools/Crowd Debugger")]
    public static void ShowEditorWindow()
    {
        GetWindow<CrowdDebug>("Crowd Debugger");
    }

    public void OnGUI()
    {
        NPCSpawner spawner = FindFirstObjectByType<NPCSpawner>();
        GUILayout.Label("This tool is used currently to enable and disable the mesh renderers on the crowd points to make it easier to see when placing or testing.", EditorStyles.largeLabel);
        GUILayout.Label("This tool is also used to enable and disable debug on the target to see where it is.", EditorStyles.largeLabel);
        EditorGUILayout.Space();
        if (GUILayout.Button("Enable Debug Points"))
        {
            spawner.crowdPoints.ForEach(p => p.GetComponent<CrowdPointAllocator>().points.ForEach(p2 => p2.GetComponent<MeshRenderer>().enabled = true));
            spawner.crowdPoints.ForEach(p => p.GetComponent<MeshRenderer>().enabled = true);
        }
        if (GUILayout.Button("Disable Debug Points"))
        {
            spawner.crowdPoints.ForEach(p => p.GetComponent<CrowdPointAllocator>().points.ForEach(p2 => p2.GetComponent<MeshRenderer>().enabled = false));
            spawner.crowdPoints.ForEach(p => p.GetComponent<MeshRenderer>().enabled = false);
        }
        if (Application.isPlaying)
        {
            //for code that may break things when not running

            if (spawner != null && spawner.Target != null)
            {
                MeshRenderer targetMesh = spawner.Target.GetComponent<MeshRenderer>();

                if (GUILayout.Button("Enable Debug Target Visual"))
                {
                    if (targetDebugMaterial == null)
                    {
                        targetDebugMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                        targetDebugMaterial.color = Color.red;
                    }
                    if (targetOriginalMaterial == null)
                    {
                        targetOriginalMaterial = targetMesh.material;
                    }

                    targetMesh.material = targetDebugMaterial;
                }
                if (GUILayout.Button("Disable Debug Target Visual"))
                {
                    targetMesh.material = targetOriginalMaterial;
                }
            }
            else
            {
                GUILayout.Label("No target in the scene", EditorStyles.boldLabel);
            }
        }        
        else
        {
            GUILayout.Label("Enter Play Mode to see more options", EditorStyles.boldLabel); //this is to stop any scary destruction of objects that could cause catastrophy
        }
    }
}
