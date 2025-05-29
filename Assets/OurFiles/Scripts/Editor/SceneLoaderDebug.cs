using UnityEditor;
using UnityEngine;

/// <summary>
/// Used to test loading of scenes
/// </summary>
public class SceneLoaderDebug : EditorWindow
{
    private string sceneName;
    
    [MenuItem("Tools/Scene Loader")]
    static void ShowEditorWindow()
    {
        GetWindow<SceneLoaderDebug>("Scene Loader");
    }

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("Enter play mode to test");
            return;
        }

        SceneLoader loader = FindFirstObjectByType<SceneLoader>();
        if (loader == null)
        {
            Debug.Log("Did not find a scene loader. Making a new one.");
            loader = new GameObject("Scene Loader").AddComponent<SceneLoader>();
        }

        sceneName = EditorGUILayout.TextField("Scene name", sceneName);
        if (GUILayout.Button("Load scene"))
        {
            loader.LoadScene(sceneName);
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Main menu"))
        {
            loader.LoadMenuScene();
        }
        if (GUILayout.Button("Win game"))
        {
            loader.LoadGameWon();
        }
        if (GUILayout.Button("Lose game"))
        {
            loader.LoadGameLost();
        }
    }
}
