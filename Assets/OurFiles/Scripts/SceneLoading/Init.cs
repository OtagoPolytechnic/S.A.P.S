using UnityEngine;

/// <summary>
/// Loads the first scene of the game
/// </summary>
public class Init : MonoBehaviour
{
    [SerializeField] string sceneOnStart = "MainMenu";

    void Start()
    {
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(sceneOnStart));
    }
}
