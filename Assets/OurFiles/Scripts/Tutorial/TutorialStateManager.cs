using UnityEngine;

public class TutorialStateManager : Singleton<TutorialStateManager>
{
    [SerializeField]
    private SceneLoader sceneLoader;

    public void ResetStage(int stage)
    {
        sceneLoader.LoadScene("Tutorial");
    }
}
