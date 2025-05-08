using UnityEngine;

public class ContractCardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardInHand;
    [SerializeField] private ContractInfoCard cardInScene;
    [SerializeField] private GameObject[] leftControllerVisuals;
    [SerializeField] private PauseManager pauseManager;
    
    private bool isCardVisible = true; 
    private bool cardVisibleBeforePause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardInScene.onGrab.AddListener(HandleGrab);
        pauseManager.PauseChange.AddListener(HandlePause);
    }

    // Toggles the left controller visuals for the contract card visuals
    public void ToggleVision() 
    {
        isCardVisible = !isCardVisible;
        foreach (GameObject mesh in leftControllerVisuals)
        {
            mesh.SetActive(!isCardVisible);
        }
        cardInHand.SetActive(isCardVisible);
    }

    // Destroys the in scene card and enables the card in hand
    private void HandleGrab() 
    {
        Destroy(cardInScene.gameObject);
        ToggleVision();
    }

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
