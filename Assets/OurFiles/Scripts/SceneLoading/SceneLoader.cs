using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Global singleton to smoothly and asynchronously load scenes
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private string menuScene = "MainMenu";
    [SerializeField] private string gameLostScene;
    [SerializeField] private string gameWonScene;
    [SerializeField] private Material blackFadeMaterial;
    [SerializeField, Range(0.2f, 10)] private float fadeSpeed;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Fades to black and loads the scene that matches the given name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    /// <summary>
    /// Shortcut to load the main menu, without requiring 
    /// </summary>
    public void LoadMenuScene() => LoadSceneWithFade(menuScene);

    /// <summary>
    /// Loads game lost scene and passes information from <c>Contract</c>
    /// </summary>
    public void LoadGameLost()
    {
        LoadSceneWithFade(gameLostScene);
    }

    /// <summary>
    /// Loads game won scene and passes information from <c>Contract</c>
    /// </summary>
    public void LoadGameWon()
    {
        LoadSceneWithFade(gameWonScene);
    }

    /// <summary>
    /// Fades to black and loads the scene that matches the given name
    /// </summary>
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log($"Loaded Scene {sceneName}");
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        yield return StartCoroutine(Fade(1));
        yield return StartCoroutine(LoadSceneAsync(sceneName));
        yield return StartCoroutine(Fade(0));
    }

    /// <summary>
    /// Fades the overlay layer on the player camera to the given value
    /// </summary>
    public IEnumerator Fade(int alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        int direction = alpha > blackFadeMaterial.color.a ? 1 : -1;
        while (blackFadeMaterial.color.a != alpha)
        {
            blackFadeMaterial.color = new Color()
            {
                a = Mathf.Clamp(blackFadeMaterial.color.a + direction * fadeSpeed * Time.deltaTime, 0, 1)
            };
            yield return null;
        }
    }
}
