using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// A suite of popular tools to aid gameplay testing
/// </summary>
public class GameplayTestTools : EditorWindow
{
    const string xrSimulatorPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.0.7/XR Device Simulator/XR Device Simulator.prefab";
    const string initScenePath = "Assets/OurFiles/Scenes/Init.unity";

    private Vector2 scrollPosition;

    private bool showGeneralSettings = true;
    private XRDeviceSimulator simulator;
    private bool useXRSimulator;
    private bool reloadActiveScene;
    private bool loadInitScene;

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
        TargetNPCSettings();
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Displays general settings in a foldout header 
    /// </summary>
    void GeneralSettings()
    {
        EditorGUILayout.Space();
        showGeneralSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGeneralSettings, "General");

        if (showGeneralSettings)
        {
            useXRSimulator = EditorGUILayout.Toggle("Use XR Device Simulator", useXRSimulator);
            loadInitScene = EditorGUILayout.Toggle("Load Init scene on play", loadInitScene);
            reloadActiveScene = GUILayout.Button("Restart scene");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyGeneralSettings();
    }

    /// <summary>
    /// Displays settings to do with the target NPC in a foldout header
    /// </summary>
    void TargetNPCSettings()
    {
        EditorGUILayout.Space();
        showTargetNPCSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showTargetNPCSettings, "Target NPC");

        if (showTargetNPCSettings)
        {
            enableTargetBeacon = EditorGUILayout.Toggle("Display beacon", enableTargetBeacon);
            killTarget = GUILayout.Button("Kill");
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyTargetNPCSettings();
    }

    /// <summary>
    /// Does all of the things the settings say it should do
    /// </summary>
    void ApplyGeneralSettings()
    {
        EditorSceneManager.playModeStartScene = loadInitScene ?
            AssetDatabase.LoadAssetAtPath<SceneAsset>(initScenePath) : null;

        if (!Application.isPlaying) return;

        if (simulator == null)
        {
            simulator = Instantiate(AssetDatabase.LoadAssetAtPath<XRDeviceSimulator>(xrSimulatorPrefabPath));
            DontDestroyOnLoad(simulator);
        }

        simulator.gameObject.SetActive(useXRSimulator);

        if (reloadActiveScene) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Does all of the things the settings say it should do
    /// </summary>
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
