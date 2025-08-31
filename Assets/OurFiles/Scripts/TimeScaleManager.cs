using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeScaleManager : Singleton<TimeScaleManager>
{
    void Start()
    {
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
        DontDestroyOnLoad(this);
    }

    void HandleActiveSceneChanged(Scene previous, Scene active)
    {
        Time.timeScale = 1;
    }
}
