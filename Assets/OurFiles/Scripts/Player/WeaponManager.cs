using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

// Base written by: Christian Irvine

/// <summary>
/// Manages weapon visibility and toggling between the weapon and controller visuals.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private InputActionManager inputActionManager;

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject[] rightControllerVisuals;

    private bool isEnabled = false;

    /// <summary>
    /// Gets or sets whether the weapon is enabled.
    /// Triggers <see cref="EnableWeaponChange"/> when updated.
    /// </summary>
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            if (weapon != null)
            {
                weapon.SetActive(value);
                EnableWeaponChange?.Invoke(value);
            }
        }
    }

    public UnityEvent<bool> EnableWeaponChange = new UnityEvent<bool>();

    /// <summary>
    /// Toggles the weapon on or off when the input action is performed. 
    /// Hides controller visuals when the weapon is active.
    /// </summary>
    public void ToggleWeapon(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            IsEnabled = !IsEnabled;
            foreach (GameObject mesh in rightControllerVisuals)
            {
                mesh.SetActive(!IsEnabled);
            }
        }
    }
}
