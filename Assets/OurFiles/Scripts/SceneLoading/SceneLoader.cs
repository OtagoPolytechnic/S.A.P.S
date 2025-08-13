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

    private Material fadeMatInstance; // changed this to an instance to avoid shared material issues

    protected override void Awake()
    {
        // create a material instance so original asset is not modified
        fadeMatInstance = new Material(blackFadeMaterial);

        // reset alpha at start so fade is transparent on launch
        Color c = fadeMatInstance.color;
        c.a = 0;
        fadeMatInstance.color = c;

        DontDestroyOnLoad(gameObject);
    }

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
    public IEnumerator Fade(int targetAlpha)
    {
        targetAlpha = Mathf.Clamp(targetAlpha, 0, 1);

        int direction = targetAlpha > fadeMatInstance.color.a ? 1 : -1;
        //Floats often cannot represent decimal values exactly so changed to approximate value to remove infinite loops
        while (!Mathf.Approximately(fadeMatInstance.color.a, targetAlpha)) 
        {
            Color c = fadeMatInstance.color; // preserve original RGB
            c.a = Mathf.Clamp(c.a + direction * fadeSpeed * Time.deltaTime, 0, 1); 
            fadeMatInstance.color = c; // assign back 
            yield return null;
        }
    }

}
