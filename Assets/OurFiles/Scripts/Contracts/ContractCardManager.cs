using System.Collections;
using UnityEngine;

// Base written by Jenna 

public class ContractCardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardInHand;
    [SerializeField] private ContractInfoCard cardInScene;
    [SerializeField] private GameObject[] leftControllerVisuals;
    [SerializeField] private PauseManager pauseManager;
    
    private bool isCardVisible = false; 
    private bool cardVisibleBeforePause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // objects not on the player need to be found on new scene loads
        if (cardInScene == null) cardInScene = FindFirstObjectByType<ContractInfoCard>();
        if (pauseManager == null) pauseManager = FindFirstObjectByType<PauseManager>();
        
        // stop null exceptions in scenes that shouldnt have cardInScene or pause (eg main menu)
        if (cardInScene != null) cardInScene.onGrab.AddListener(() => StartCoroutine(HandleGrab()));
        if (pauseManager != null) pauseManager.PauseChange.AddListener(HandlePause);
    }

    // Toggles the left controller visuals for the contract card visuals
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
}
