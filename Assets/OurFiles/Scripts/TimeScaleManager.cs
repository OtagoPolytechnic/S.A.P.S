using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures that <c>Time.timeScale</c> is reset to normal whenever a new scene loads.
/// This prevents unintended slow-motion or paused states from persisting across scenes.
/// </summary>
public class TimeScaleManager : Singleton<TimeScaleManager>
{
    void Start()
    {
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Resets the time scale to 1 when the active scene changes.
    /// </summary>
    /// <param name="previous">The scene being unloaded.</param>
    /// <param name="active">The newly active scene.</param>
    void HandleActiveSceneChanged(Scene previous, Scene active)
    {
        Time.timeScale = 1;
    }
}
