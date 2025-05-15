using UnityEngine;

public class TutorialLeave : MonoBehaviour
{
    private string mainMenu = "MainMenuScene";
    private string gameScene = "city-01";
    [SerializeField]
    private bool isGameExit;
    private SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isGameExit)
            {
                sceneLoader.LoadScene(gameScene);
            }
            else
            {
                sceneLoader.LoadScene(mainMenu);
            }
        }
    }
}
