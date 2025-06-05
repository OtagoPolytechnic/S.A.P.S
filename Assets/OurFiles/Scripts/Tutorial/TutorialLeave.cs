using System.Collections;
using UnityEngine;

public class TutorialLeave : MonoBehaviour
{
    private string mainMenu = "MainMenuScene";
    private string gameScene = "city-01";
    [SerializeField]
    private bool isGameExit; //referring to entering the playable game

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isGameExit)
            {
                SceneLoader.Instance.LoadScene(gameScene);
            }
            else
            {
                SceneLoader.Instance.LoadMenuScene();
            }
            Contract.Instance.EndContract();
        }
    }
}
