using UnityEngine;

//Written by Rohan Anakin
/// <summary>
/// Handles turning on the tutorial panels for the tutorial scene
/// </summary>
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
