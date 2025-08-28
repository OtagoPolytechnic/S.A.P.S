using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// A suite of popular tools to aid gameplay testing
/// </summary>
public class GameplayTestTools : EditorWindow
{
    private Vector2 scrollPosition;

    private bool showXRSection = true;
    private XRDeviceSimulator simulator;
    private bool useXRSimulator;

    private bool showTargetHelper = true;
    private GameObject targetNPC;
    private GameObject beacon;
    private bool enableTargetBeacon;

    [MenuItem("Tools/Gameplay Test Tools")]
    static void ShowEditorWindow()
    {
        GetWindow<GameplayTestTools>("Gameplay Test Tools");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditXRSimulator();
        TargetNPCHelper();
        EditorGUILayout.EndScrollView();
    }

    void EditXRSimulator()
    {
        EditorGUILayout.Space();
        showXRSection = EditorGUILayout.BeginFoldoutHeaderGroup(showXRSection, "XR");

        if (showXRSection)
        {
            useXRSimulator = EditorGUILayout.Toggle("Use XR Device Simulator", useXRSimulator);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        
        if (!Application.isPlaying) return; 

        if (simulator == null)
        {
            simulator = Instantiate(AssetDatabase.LoadAssetAtPath<XRDeviceSimulator>(
                "Assets/Samples/XR Interaction Toolkit/3.0.7/XR Device Simulator/XR Device Simulator.prefab"
            ));
            DontDestroyOnLoad(simulator);
        }

        simulator.gameObject.SetActive(useXRSimulator);
    }

    void TargetNPCHelper()
    {
        EditorGUILayout.Space();
        showTargetHelper = EditorGUILayout.BeginFoldoutHeaderGroup(showTargetHelper, "Target NPC");

        if (showTargetHelper)
        {
            enableTargetBeacon = EditorGUILayout.Toggle("Display beacon", enableTargetBeacon);
        }

        if (!Application.isPlaying) return;

        if (targetNPC == null) targetNPC = GameObject.Find("TargetNPC");

        if (enableTargetBeacon)
        {
            if (beacon == null) 
            {
                beacon = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/OurFiles/prefabs/Beacon.prefab"
                ));
                beacon.transform.SetParent(targetNPC.transform);
                beacon.transform.localPosition = Vector3.zero;
            }
            beacon.SetActive(true);
        }
        else
        {
            beacon.SetActive(false);
        }
    }
}
