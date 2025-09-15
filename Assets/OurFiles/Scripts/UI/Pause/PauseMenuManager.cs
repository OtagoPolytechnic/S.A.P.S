using UnityEngine;
using TMPro;

/// <summary>
/// Manages the UI of the pause menu when its enabled.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Transform cam;

    [SerializeField] private GameObject normalMenuContent;
    [SerializeField] private GameObject settingsMenuContent;
    [SerializeField] private TMP_Text menuTitle;
    

    private void Start()
    {
        PauseManager.Instance.PauseChange.AddListener(OnPauseChange);

        OnPauseChange(PauseManager.Instance.State);
    }

    private void OnPauseChange(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            gameObject.SetActive(true);
            Vector3 newPos = new Vector3(cam.position.x, 0, cam.position.z) + (cam.transform.forward * 1.5f);
            transform.position = new Vector3(newPos.x, cam.transform.position.y, newPos.z);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void MainMenu()
    {
        SceneLoader.Instance.LoadMenuScene();
    }

    public void Resume()
    {
        PauseManager.Instance.State = PauseState.Play;
        
    }

    public void Settings()
    {
        bool isOpeningSettings = !settingsMenuContent.activeSelf;
        settingsMenuContent.SetActive(isOpeningSettings);
        normalMenuContent.SetActive(!isOpeningSettings);
        menuTitle.text = isOpeningSettings ? "SETTINGS" : "PAUSED";
    }


}
