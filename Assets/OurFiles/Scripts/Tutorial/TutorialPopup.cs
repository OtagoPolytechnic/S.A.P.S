using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField]
    GameObject tutorialPanel;
    bool enterCheck;

    void Start()
    {
        tutorialPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enterCheck)
        {
            tutorialPanel.SetActive(true);
            enterCheck = true;
        }
    }
}
