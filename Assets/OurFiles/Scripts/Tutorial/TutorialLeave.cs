using System.Collections;
using UnityEngine;

//Written by Rohan Anakin
/// <summary>
/// Handles the exiting from the scene in the tutorial
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
                print("Leaving!!!!!!");
                SceneLoader.Instance.LoadMenuScene();
            }
        }
    }

    /// <summary>
    /// Waits until the close door coroutine has finished before calling the load scene method
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForClosedDoors()
    {
        yield return StartCoroutine(elevator.CloseDoors());
        SceneLoader.Instance.LoadScene(gameScene);
    }
}
