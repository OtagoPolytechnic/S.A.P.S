using UnityEngine;

/// <summary>
/// Entry point that loads the first scene when the game starts.
/// In editor mode, can override with the last active scene.
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
