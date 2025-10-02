using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Global singleton to fade in/out and load scenes asynchronously.
/// Provides shortcuts for menu, win, and loss scenes.
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private string menuScene = "MainMenu";
    [SerializeField] private string gameLostScene;
    [SerializeField] private string gameWonScene;
    [SerializeField] private Material blackFadeMaterial;
    [SerializeField, Range(0.2f, 10)] private float fadeSpeed;

    public Material FadeMatInstance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        FadeMatInstance = new Material(blackFadeMaterial);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // reset alpha at start so fade is transparent on launch
        Color c = FadeMatInstance.color;
        c.a = 0;

        FadeMatInstance.color = c;
    }

    /// <summary>
    /// Fades to black and loads the scene that matches the given name
    /// </summary>
    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneWithFade(sceneName));

    public void LoadMenuScene() => StartCoroutine(LoadSceneWithFade(menuScene));

    public void LoadGameLost() => StartCoroutine(LoadSceneWithFade(gameLostScene));

    public void LoadGameWon() => StartCoroutine(LoadSceneWithFade(gameWonScene));

    /// <summary>
    /// Loads a scene without fade, waits for completion.
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
    /// Smoothly changes overlay alpha to target value.
    /// </summary>
    public IEnumerator Fade(float targetAlpha)
    {
        targetAlpha = Mathf.Clamp01(targetAlpha);

        while (!Mathf.Approximately(FadeMatInstance.color.a, targetAlpha))
        {
            Color c = FadeMatInstance.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
            FadeMatInstance.color = c;
            yield return null;
        }
    }
}
