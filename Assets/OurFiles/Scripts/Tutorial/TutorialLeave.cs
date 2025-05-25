using System.Collections;
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
            if (isGameExit)
            {
                StartCoroutine(TutorialSceneWait(gameScene));
            }
            else
            {
                StartCoroutine(TutorialSceneWait(mainMenu));
            }
        }
    }

    IEnumerator TutorialSceneWait(string scene)
    {
        yield return new WaitForSecondsRealtime(3f);
        sceneLoader.LoadScene(scene);
    }
}
