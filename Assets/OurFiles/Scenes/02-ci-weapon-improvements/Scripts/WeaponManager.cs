using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private InputActionManager inputActionManager;

    [SerializeField] private GameObject weapon;

    private bool isEnabled = false;

    public bool IsEnabled 
    { 
        get =>  isEnabled; 
        set 
        {
            isEnabled = value;
            weapon.SetActive(value);
        }
    }

    private void Start()
    {
        InputActionAsset asset = inputActionManager.actionAssets[0];
        InputActionMap actionMap = asset.FindActionMap("XRI Right Interaction");
        InputAction action = actionMap.FindAction("Menu");
        action.performed += context => ToggleWeapon();
    }

    public void ToggleWeapon()
    {
        IsEnabled = !IsEnabled;
    }
}
