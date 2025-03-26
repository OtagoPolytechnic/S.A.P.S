using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

//Base written by: Jenna Boyes

/// <summary>
/// This script is required to pretend to be a LocomotionProvider.
/// To allow control over the Coherency TunnelingVignetteController 
/// doc: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.html
/// </summary>
public class CoherencyVignette : LocomotionProvider
{

    public new bool isLocomotionActive { get; private set; }
    public new LocomotionState locomotionState { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLocomotionActive = true;
        locomotionState = LocomotionState.Moving;
    }

    public void Show()
    {
        isLocomotionActive = true;
        locomotionState = LocomotionState.Moving;
    }

    public void Hide()
    {
        isLocomotionActive = false;
        locomotionState = LocomotionState.Idle;
    }
}
