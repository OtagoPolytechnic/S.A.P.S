using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// A suite of popular tools to aid gameplay testing
/// </summary>
public class GameplayTestTools : EditorWindow
{
    const string xrSimulatorPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.0.7/XR Device Simulator/XR Device Simulator.prefab";

    private Vector2 scrollPosition;

    private bool showGeneralSettings = true;
    private XRDeviceSimulator simulator;
    private bool useXRSimulator = true;
    private bool reloadActiveScene;
    private bool loadInitSceneOnPlay = true;

    private bool showNPCSettings = true;
    private NPCSpawner npcSpawner;
    private bool displayCrowdPoints;

    private bool showTargetNPCSettings = true;
    private GameObject targetNPC;
    private GameObject targetBeacon;
    private bool enableTargetBeacon;
    private bool killTarget;

    [MenuItem("Tools/Gameplay Test Tools")]
    static void ShowEditorWindow()
    {
        GetWindow<GameplayTestTools>("Gameplay Test Tools");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GeneralSettings();
        NPCSettings();
        EditorGUILayout.EndScrollView();
    }

    void GeneralSettings()
    {
        EditorGUILayout.Space();
        showGeneralSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGeneralSettings, "General");

        if (showGeneralSettings)
        {
            useXRSimulator = EditorGUILayout.Toggle("Use XR Device Simulator", useXRSimulator);
            loadInitSceneOnPlay = EditorGUILayout.Toggle("Load Init scene on play", loadInitSceneOnPlay);
            reloadActiveScene = GUILayout.Button("Restart scene");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyGeneralSettings();
    }

    void NPCSettings()
    {
        EditorGUILayout.Space();
        showNPCSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showNPCSettings, "NPCs");

        if (showNPCSettings)
        {
            displayCrowdPoints = EditorGUILayout.Toggle("Display crowd points", displayCrowdPoints);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        if (showNPCSettings) // we aren't allowed to nest foldout header groups, this is how i pretend that we can
        {
            TargetNPCSettings();
        }

        ApplyNPCSettings();
    }

    void TargetNPCSettings()
    {
        showTargetNPCSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTargetNPCSettings, "Target NPC");

        if (showTargetNPCSettings)
        {
            enableTargetBeacon = EditorGUILayout.Toggle("Display beacon", enableTargetBeacon);
            killTarget = GUILayout.Button("Kill");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyTargetNPCSettings();
    }

    void GuardNPCSettings()
    {
        showTargetNPCSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTargetNPCSettings, "Target NPC");

        if (showTargetNPCSettings)
        {
            enableTargetBeacon = EditorGUILayout.Toggle("Display beacon", enableTargetBeacon);
            killTarget = GUILayout.Button("Kill");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyTargetNPCSettings();
    }

    void ApplyGeneralSettings()
    {
        EditModeSceneLoader.LoadInitSceneOnPlay = loadInitSceneOnPlay;

        if (!Application.isPlaying) return;

        if (simulator == null)
        {
            simulator = Instantiate(AssetDatabase.LoadAssetAtPath<XRDeviceSimulator>(xrSimulatorPrefabPath));
            DontDestroyOnLoad(simulator);
        }

        simulator.gameObject.SetActive(useXRSimulator);

        if (reloadActiveScene) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ApplyNPCSettings()
    {
        if (npcSpawner == null) npcSpawner = FindFirstObjectByType<NPCSpawner>();

        npcSpawner.crowdPoints.ForEach(p => p.GetComponent<CrowdPointAllocator>().points.ForEach(p2 => p2.GetComponent<MeshRenderer>().enabled = displayCrowdPoints));
        npcSpawner.crowdPoints.ForEach(p => p.GetComponent<MeshRenderer>().enabled = displayCrowdPoints);

        if (!Application.isPlaying) return;
    }

    void ApplyTargetNPCSettings()
    {
        if (!Application.isPlaying) return;

        if (targetNPC == null) targetNPC = GameObject.Find("TargetNPC");

        if (targetBeacon == null)
        {
            targetBeacon = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/OurFiles/prefabs/Beacon.prefab"
            ));
            targetBeacon.transform.SetParent(targetNPC.transform);
            targetBeacon.transform.localPosition = Vector3.zero;
        }
        targetBeacon.SetActive(enableTargetBeacon);

        if (killTarget && targetNPC != null)
        {
            targetNPC.GetComponent<Hurtbox>().Health = 0;
        }
    }
}
