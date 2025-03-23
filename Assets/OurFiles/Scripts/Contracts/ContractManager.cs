using UnityEngine;

// base written by Joshii

// TODO: make the summary more descriptive
/// <summary>
/// Manages the contracts in the game
/// </summary>
public class ContractManager : MonoBehaviour
{
    public static ContractManager Instance { get; private set; }
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    void Update()
    {
        
    }
}
