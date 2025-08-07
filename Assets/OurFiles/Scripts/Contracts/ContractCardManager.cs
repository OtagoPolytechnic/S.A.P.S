using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Base written by Jenna 

public class ContractCardManager : MonoBehaviour
{
    [Header("In Hand Card")]
    [SerializeField] private GameObject cardInHand;
    [SerializeField] private Image inHandTargetView;
    [SerializeField] private TMP_Text inHandMissionInfo;

    [Header("In Scene Card")] 
    private ContractInfoCard cardInScene;
    [SerializeField] private Image inSceneTargetView;
    [SerializeField] private TMP_Text inSceneMissionInfo;

    [Space]
    [SerializeField] private string targetKilledContractText;
    [SerializeField] private GameObject[] leftControllerVisuals;
    [SerializeField] private PauseManager pauseManager;

    private bool isCardVisible = false;
    /// <summary>
    /// In-hand Contract card visibility
    /// </summary>
    public bool IsCardVisible => isCardVisible;
    private bool cardVisibleBeforePause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // objects not on the player need to be found on new scene loads
        if (!cardInScene) cardInScene = FindFirstObjectByType<ContractInfoCard>();
        if (!pauseManager) pauseManager = FindFirstObjectByType<PauseManager>();

        // stop null exceptions in scenes that shouldnt have cardInScene or pause (eg main menu)
        if (cardInScene) cardInScene.onGrab.AddListener(() => StartCoroutine(HandleGrab()));
        if (pauseManager) pauseManager.PauseChange.AddListener(HandlePause);
    }

    /// <summary>
    /// Toggles the left controller visuals for the contract card visuals
    /// </summary>
    public void ToggleVision()
    {
        if (!cardInScene)
        {
            isCardVisible = !isCardVisible;
            foreach (GameObject mesh in leftControllerVisuals)
            {
                mesh.SetActive(!isCardVisible);
            }
            cardInHand.SetActive(isCardVisible);
        }
    }

    // Destroys the in scene card and enables the card in hand
    private IEnumerator HandleGrab()
    {
        Destroy(cardInScene.gameObject);
        yield return null; //wait 1 frame for scene card to destroy
        if (!isCardVisible) ToggleVision();
    }

    // Toggles the display of the contract card when pause menu is opened/closed
    private void HandlePause(PauseState state)
    {
        if (state == PauseState.Paused && isCardVisible)
        {
            ToggleVision();
            cardVisibleBeforePause = true;
        }
        else if (state == PauseState.Play && cardVisibleBeforePause)
        {
            ToggleVision();
            cardVisibleBeforePause = false; // stop toggle next time menu is closed and card wasnt visible
        }
    }

    /// <summary>
    /// Set the mission info text on the contract card
    /// </summary>
    /// <param name="newMissionInfo"></param>
    public void ChangeCardInfo(string newMissionInfo)
    {
        inHandMissionInfo.text = newMissionInfo;

        if (cardInScene)
        {
            inSceneMissionInfo.text = newMissionInfo;
        }
    }

    /// <summary>
    /// Change the info text on the contract card to state the target is dead
    /// </summary>
    public void SetCardInfoToTargetKilled()
    {
        ChangeCardInfo(targetKilledContractText);
    }

    /// <summary>
    /// Toggles the visibility of the Target spinning on Cards 
    /// </summary>
    public void ToggleTargetCamera()
    {
        inHandTargetView.enabled = !inHandTargetView.enabled;

        if (cardInScene)
        {
            inSceneTargetView.enabled = !inSceneTargetView.enabled;
        }
    }
}
