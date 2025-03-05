using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private SceneLoader loader;
    [SerializeField] private string sceneOnPlay;

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
            transform.position = new Vector3(cam.position.x, cam.position.y - 0.25f, cam.position.z) + (transform.forward * 1.5f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void MainMenu()
    {
        loader.LoadScene(sceneOnPlay);
    }

    public void Resume()
    {
        PauseManager.Instance.State = PauseState.Play;
    }
}
