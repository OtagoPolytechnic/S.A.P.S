using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Loads the first scene of the game
/// </summary>
public class Init : MonoBehaviour
{
    [SerializeField] string sceneOnStart = "MainMenu";

    void Start()
    {
#if UNITY_EDITOR
        if (EditModeSceneLoader.LoadInitSceneOnPlay && EditModeSceneLoader.ActiveSceneInEditMode != "Init")
        {
            sceneOnStart = EditModeSceneLoader.ActiveSceneInEditMode;
        }
#endif
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(sceneOnStart));
    }
}
