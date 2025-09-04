using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditModeSceneLoader
{
    const string initScenePath = "Assets/OurFiles/Scenes/Init.unity";

    private static string activeSceneInEditMode;

    private static bool loadInitSceneOnPlay;
    public static bool LoadInitSceneOnPlay
    {
        get => loadInitSceneOnPlay; set
        {
            loadInitSceneOnPlay = value;
            EditorSceneManager.playModeStartScene = loadInitSceneOnPlay ?
                AssetDatabase.LoadAssetAtPath<SceneAsset>(initScenePath) : null;
        }
    }

    /// <summary>
    /// Class initializer. Adds listeners to active scene and play mode state changes
    /// </summary>
    static EditModeSceneLoader()
    {
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChangedInEditMode;
        activeSceneInEditMode = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Stores the scene that is most recently loaded in edit mode
    /// </summary>
    static void HandleActiveSceneChangedInEditMode(Scene previous, Scene active)
    {
        activeSceneInEditMode = active.name;
    }

    /// <summary>
    /// Runs when entering/exiting play mode
    /// </summary>
    static void HandlePlayModeStateChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            #pragma warning disable CS4014 // disable warning saying that we aren't awaiting (we don't need to be)
            if (loadInitSceneOnPlay) LoadActiveSceneAfterInit();
            #pragma warning restore CS4014 
        }
    }

    /// <summary>
    /// Waits until init scene has run, then loads whatever scene was open in edit mode
    /// </summary>
    static async Awaitable LoadActiveSceneAfterInit() // Awaitable instead of a coroutine or invoked method as those are not available to EditorWindow
    {
        // Init loads main menu, so when we're in main menu we can leave
        while (SceneManager.GetActiveScene().name != "MainMenu" && Application.isPlaying)
        {
            await Awaitable.NextFrameAsync();
        }
        SceneManager.LoadScene(activeSceneInEditMode);
    }
}