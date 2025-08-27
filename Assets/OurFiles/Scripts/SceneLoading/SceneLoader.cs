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

    private Material fadeMatInstance; // keeping this here incase we want to revert to an instance later.

    protected override void Awake()
    {
        base.Awake();
        // removed instance to display fade however if anything else uses this material it could break.
        fadeMatInstance = blackFadeMaterial;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // reset alpha at start so fade is transparent on launch
        Color c = fadeMatInstance.color;
        c.a = 0;

        fadeMatInstance.color = c;
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
    public IEnumerator Fade(float targetAlpha)
    {
        targetAlpha = Mathf.Clamp01(targetAlpha);

        while (!Mathf.Approximately(fadeMatInstance.color.a, targetAlpha))
        {
            Color c = fadeMatInstance.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
            fadeMatInstance.color = c;
            yield return null;
        }
    }

}
