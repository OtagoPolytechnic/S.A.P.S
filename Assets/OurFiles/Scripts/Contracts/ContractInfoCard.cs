using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

// base written by Joshii

/// <summary>
/// Floats in air until grabbed by player
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class ContractInfoCard : MonoBehaviour
{
    [Header("Float Animation")]
    [SerializeField] float floatWaveStrength = 0.2f;
    [SerializeField] float floatWaveSpeed = 3.14f;
 
    [HideInInspector] public bool hasBeenGrabbed = false; 

    public UnityEvent onGrab = new();
    //public UnityEvent<bool> EnableWeaponChange = new UnityEvent<bool>();

    private float baseYPos;

    void Start()
    {
        baseYPos = transform.position.y;
    }

    void Update()
    {
        Vector3 position = transform.position;
        position.y = baseYPos + Mathf.Sin(Time.time * floatWaveSpeed) * floatWaveStrength;
        transform.position = position;
    }

    // call from the First Select Entered event from XRGrabInteractable
    public void StopFloating()
    {
        onGrab?.Invoke();  
    }
}
