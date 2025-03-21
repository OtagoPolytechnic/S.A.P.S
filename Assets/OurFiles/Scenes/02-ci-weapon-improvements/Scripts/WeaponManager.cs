using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

// Base written by: Christian Irvine

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
            if (weapon != null )
            {
                weapon.SetActive(value);
                EnableWeaponChange?.Invoke(value);
            }
        }
    }

    public UnityEvent<bool> EnableWeaponChange = new UnityEvent<bool>();

    private void Start()
    {
        InputActionAsset asset = inputActionManager.actionAssets[0];
        InputActionMap actionMap = asset.FindActionMap("XRI Right Locomotion");
        InputAction action = actionMap.FindAction("Grab Move");
        action.performed += ctx => ToggleWeapon();
    }

    public void ToggleWeapon()
    {
        IsEnabled = !IsEnabled;
    }
}
