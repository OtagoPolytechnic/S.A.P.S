using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneOnPlay;
    [SerializeField]
    private List<Button> buttons = new();

    public void StartGame()
    {
        DisableButtons();
        SceneLoader.Instance.LoadScene(sceneOnPlay);
    }

    public void StartTutorial()
    {
        DisableButtons();
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        DisableButtons();
        // If in editor disable play mode, otherwise quit normally.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        
    }

    void DisableButtons()
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }
}
