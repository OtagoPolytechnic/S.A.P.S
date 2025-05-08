using UnityEngine;

public class ContractCardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardInHand;
    [SerializeField] private ContractInfoCard cardInScene;
    [SerializeField] private GameObject[] leftControllerVisuals;
    
    private bool isCardVisible = true; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardInScene.onGrab.AddListener(HandleGrab);
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
    public void HandleGrab() 
    {
        Destroy(cardInScene.gameObject);
        ToggleVision();
    }
}
