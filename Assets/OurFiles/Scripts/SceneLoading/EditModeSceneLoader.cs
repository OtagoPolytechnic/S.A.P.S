using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]

/// <summary>
/// Ensures the Init scene is used as the entry point when pressing Play in the editor,
/// while remembering which scene was last active in edit mode.
/// </summary>
public static class EditModeSceneLoader
{
    const string initScenePath = "Assets/OurFiles/Scenes/Init.unity";

    private static string activeSceneInEditMode;
    public static string ActiveSceneInEditMode => activeSceneInEditMode;

    public static bool LoadInitSceneOnPlay
    {
        get => EditorSceneManager.playModeStartScene != null && EditorSceneManager.playModeStartScene.name == "Init";
        set
        {
            EditorSceneManager.playModeStartScene = value ?
                AssetDatabase.LoadAssetAtPath<SceneAsset>(initScenePath) : null;
        }
    }

    /// <summary>
    /// Static initializer adds listeners and stores current active scene.
    /// </summary>
    static EditModeSceneLoader()
    {
        EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChangedInEditMode;
        activeSceneInEditMode = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Updates stored scene name when switching scenes in edit mode.
    /// </summary>
    static void HandleActiveSceneChangedInEditMode(Scene previous, Scene active)
    {
        activeSceneInEditMode = active.name;
    }
}
