using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// A suite of popular tools to aid gameplay testing
/// </summary>
public class GameplayTestTools : EditorWindow
{
    const string xrSimulatorPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.0.7/XR Device Simulator/XR Device Simulator.prefab";
    const string tutorialSceneName = "Tutorial";
    const string citySceneName = "city-01";

    private Vector2 scrollPosition;

    private bool showGeneralSettings = true;
    private XRDeviceSimulator simulator;
    private bool useXRSimulator = true;
    private bool reloadActiveScene;
    private bool loadInitSceneOnPlay = true;

    private bool showNPCSettings = true;
    private NPCSpawner npcSpawner;
    private bool displayCrowdPoints;
    private bool addSuspicion;
    private int addSuspicionAmount = 5;
    private bool setSuspicion;
    private int setSuspicionAmount = 100;
    private bool freezeNavMeshAgents;

    private bool showTargetNPCSettings = true;
    private GameObject targetNPC;
    private GameObject targetBeacon;
    private bool enableTargetBeacon;
    private bool killTarget;

    private bool showGuardNPCSettings = true;
    private bool pauseGuards;

    private bool showSceneLoaderSettings = true;
    private bool loadMenu;
    private bool loadTutorial;
    private bool loadCity;
    private bool loadGameWon;
    private bool loadGameLost;
    private string customSceneToLoad;
    private bool loadCustomScene;

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
        SceneLoaderSettings();
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
            reloadActiveScene = GUILayout.Button("Reload active scene");
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
            EditorGUILayout.BeginHorizontal();
            addSuspicionAmount = EditorGUILayout.IntField(addSuspicionAmount);
            addSuspicion = GUILayout.Button("Add suspicion");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            setSuspicionAmount = EditorGUILayout.IntField(setSuspicionAmount);
            setSuspicion = GUILayout.Button("Set suspicion");
            EditorGUILayout.EndHorizontal();
            freezeNavMeshAgents = EditorGUILayout.Toggle(
                new GUIContent("Freeze all NavMesh agents", "Disables every agent in the scene. Re-enabling does not guarantee they will continue pathing."),
                freezeNavMeshAgents);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        if (showNPCSettings) // we aren't allowed to nest foldout header groups, this is how i pretend that we can
        {
            TargetNPCSettings();
            GuardNPCSettings();
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
        showGuardNPCSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showGuardNPCSettings, "Guards");

        if (showGuardNPCSettings)
        {
            pauseGuards = EditorGUILayout.Toggle("Pause guards", pauseGuards);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplyGuardNPCSettings();
    }

    void SceneLoaderSettings()
    {
        EditorGUILayout.Space();
        showSceneLoaderSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSceneLoaderSettings, "Scene Loader");

        if (showSceneLoaderSettings)
        {
            if (!Application.isPlaying) GUILayout.Label("Enter play mode to load scenes");
            else if (SceneLoader.Instance == null) GUILayout.Label("No scene loader found! Make sure init scene has run, or create one yourself.");
            loadMenu = GUILayout.Button("Load menu");
            loadTutorial = GUILayout.Button("Load tutorial");
            loadCity = GUILayout.Button("Load city");
            loadGameWon = GUILayout.Button("Load game won");
            loadGameLost = GUILayout.Button("Load game lost");
            GUILayout.Label("Load custom scene");
            EditorGUILayout.BeginHorizontal();
            customSceneToLoad = EditorGUILayout.TextField(customSceneToLoad);
            loadCustomScene = GUILayout.Button("Load");
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        ApplySceneLoaderSettings();
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
        if (npcSpawner == null) return;

        npcSpawner.crowdPoints.ForEach(p => p.GetComponent<CrowdPointAllocator>().points.ForEach(p2 => p2.GetComponent<MeshRenderer>().enabled = displayCrowdPoints));
        npcSpawner.crowdPoints.ForEach(p => p.GetComponent<MeshRenderer>().enabled = displayCrowdPoints);

        if (!Application.isPlaying) return;

        if (addSuspicion)
        {
            foreach (VisionBehaviour vb in FindObjectsByType<VisionBehaviour>(FindObjectsSortMode.None))
            {
                vb.Suspicion += addSuspicionAmount;
            }
        }

        if (setSuspicion)
        {   // repeat code bc I don't want to run FindObjectByType all the time in OnGUI()
            foreach (VisionBehaviour vb in FindObjectsByType<VisionBehaviour>(FindObjectsSortMode.None))
            {
                vb.Suspicion = setSuspicionAmount;
            }
        }

        foreach (NavMeshAgent agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            agent.enabled = !freezeNavMeshAgents;
        }
    }

    void ApplyTargetNPCSettings()
    {
        if (!Application.isPlaying) return;

        if (targetNPC == null) targetNPC = GameObject.Find("TargetNPC");
        if (targetNPC == null) return;

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

    void ApplyGuardNPCSettings()
    {
        if (!Application.isPlaying) return;
        
        foreach (GuardLeader guard in FindObjectsByType<GuardLeader>(FindObjectsSortMode.None))
        {
            guard.enabled = !pauseGuards;
        }
        foreach (GuardFollower guard in FindObjectsByType<GuardFollower>(FindObjectsSortMode.None))
        {
            guard.enabled = !pauseGuards;
        }
    }

    void ApplySceneLoaderSettings()
    {
        if (loadMenu) SceneLoader.Instance.LoadMenuScene();
        if (loadTutorial) SceneLoader.Instance.LoadScene(tutorialSceneName);
        if (loadCity) SceneLoader.Instance.LoadScene(citySceneName);
        if (loadGameWon) SceneLoader.Instance.LoadGameWon();
        if (loadGameLost) SceneLoader.Instance.LoadGameLost();
        if (loadCustomScene) SceneLoader.Instance.LoadScene(customSceneToLoad);
    }
}
