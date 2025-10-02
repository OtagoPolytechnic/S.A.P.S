using System.Collections;
using UnityEngine;

//Written by Rohan Anakin

/// <summary>
/// Handles leaving the tutorial: either to main menu or into the game.
/// </summary>
public class TutorialLeave : MonoBehaviour
{
    private string gameScene = "city-01";
    [SerializeField]
    private bool isGameExit; //referring to entering the playable game
    [SerializeField]
    private Elevator elevator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            if (isGameExit)
            {
                StartCoroutine(WaitForClosedDoors());
            }
            else
            {
                SceneLoader.Instance.LoadMenuScene();
            }
        }
    }

    /// <summary>
    /// Waits for doors to close before loading the game scene.
    /// </summary>
    IEnumerator WaitForClosedDoors()
    {
        yield return StartCoroutine(elevator.CloseDoors());
        SceneLoader.Instance.LoadScene(gameScene);
    }
}
