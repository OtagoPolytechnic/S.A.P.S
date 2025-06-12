using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneOnPlay;
    [SerializeField]
    private List<Button> buttons = new();

    void Start()
    {
        StartCoroutine(Wait());
    }
    
    IEnumerator Wait()
    {
        ToggleButtons(false);
        yield return new WaitForSecondsRealtime(2);
        ToggleButtons(true);
    }

    public void StartGame()
    {
        ToggleButtons(false);
        SceneLoader.Instance.LoadScene(sceneOnPlay);
    }

    public void StartTutorial()
    {
        ToggleButtons(false);
        SceneLoader.Instance.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        ToggleButtons(false);
        // If in editor disable play mode, otherwise quit normally.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    void ToggleButtons(bool state)
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = state;
        }
    }
}
