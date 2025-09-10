using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditModeSceneLoader
{
    const string initScenePath = "Assets/OurFiles/Scenes/Init.unity";

    private static string activeSceneInEditMode;
    public static string ActiveSceneInEditMode => activeSceneInEditMode;

    public static bool LoadInitSceneOnPlay
    {
        get => EditorSceneManager.playModeStartScene.name == "Init";
        set
        {
            Debug.Log("Set play mode start scene to " + (value ? "Init" : "null"));
            EditorSceneManager.playModeStartScene = value ?
                AssetDatabase.LoadAssetAtPath<SceneAsset>(initScenePath) : null;
        }
    }

    /// <summary>
    /// Class initializer. Adds listeners to active scene and play mode state changes
    /// </summary>
    static EditModeSceneLoader()
    {
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
}
