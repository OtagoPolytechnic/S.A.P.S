using UnityEngine;

public class TutorialCheckConsumer : MonoBehaviour
{
    TutorialStateManager stateManager;
    void Start()
    {
        GameObject manager = GameObject.Find("TutorialStateManager");
        stateManager = manager.GetComponent<TutorialStateManager>();
    }

    void OnDestroy()
    {
        stateManager.StartInit();
    }
}
