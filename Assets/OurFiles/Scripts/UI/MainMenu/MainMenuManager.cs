using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] protected SceneLoader loader;
    [SerializeField] private string sceneOnPlay;

    public void StartGame()
    {
        loader.LoadScene(sceneOnPlay);
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
