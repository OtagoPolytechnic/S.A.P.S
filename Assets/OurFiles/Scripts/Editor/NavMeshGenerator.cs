using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
/// <summary>
/// This tool allows you to easily bake the navmesh
/// </summary>
public class NavMeshGenerator : EditorWindow
{
    [MenuItem("Tools/NavMesh Generator")]
    public static void ShowEditorWindow()
    {
        GetWindow<NavMeshGenerator>("NavMesh Generator");
    }

    public void OnGUI()
    {
        int InvisibleLayer = LayerMask.NameToLayer("Invisible");
        GUILayout.Label("This tool is used to generate the nav mesh in which the NPC's will walk on.", EditorStyles.largeLabel);
        GUILayout.Label("This tool works by finding every mesh layered with the layer 'Invisible' then enabling their mesh renderer, baking the scene then disabling their renderer.");
        GUILayout.Space(0);
        GUILayout.Label("Please remember to layer your objects you want to be included in the bake but not visible with the above layer.");

        if (GUILayout.Button("Bake Scene"))
        {
            NavMeshSurface surface = FindFirstObjectByType<NavMeshSurface>();
            if (surface == null)
            {
                Debug.LogError("There is no Nav Mesh Surface object. Please add one into the scene");
                return;
            }

            //this is disabled because FindObjectsOfType is slower than the alternative but it doesn't matter cause this never happens at runtime
            #pragma warning disable CS0618 // Type or member is obsolete
            IEnumerable<GameObject> allObjects = FindObjectsOfType<GameObject>().Where(g => g.layer == InvisibleLayer);
            #pragma warning restore CS0618 // Type or member is obsolete
            foreach (GameObject item in allObjects)
            {
                item.GetComponent<MeshRenderer>().enabled = true;
            }
            surface.BuildNavMesh();
            foreach (GameObject item in allObjects)
            {
                item.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
