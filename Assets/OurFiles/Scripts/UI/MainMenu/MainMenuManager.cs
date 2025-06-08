using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneOnPlay;

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene(sceneOnPlay);
    }

    public void StartTutorial()
    {
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        // If in editor disable play mode, otherwise quit normally.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
