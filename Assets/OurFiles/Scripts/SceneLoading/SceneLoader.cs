using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private string menuScene = "MainMenu";
    [SerializeField] private string gameLostScene;
    [SerializeField] private string gameWonScene;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Fades to black and loads the scene that matches the given name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Shortcut to load the main menu, without requiring 
    /// </summary>
    public void LoadMenuScene() => LoadScene(menuScene);

    /// <summary>
    /// Loads game lost scene and passes information from <c>Contract</c>
    /// </summary>
    public void LoadGameLost()
    {
        LoadScene(gameLostScene);
    }

    /// <summary>
    /// Loads game won scene and passes information from <c>Contract</c>
    /// </summary>
    public void LoadGameWon()
    {
        LoadScene(gameWonScene);
    }

    /// <summary>
    /// Fades to black and loads the scene that matches the given name
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log($"Loaded Scene {sceneName}");
    }
}
