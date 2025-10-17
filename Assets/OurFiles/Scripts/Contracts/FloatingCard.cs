using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

// base written by Joshii

/// <summary>
/// A contract card that floats in place until the player grabs it.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class FloatingCard : MonoBehaviour
{
    [Header("Float Animation")]
    [SerializeField] float floatWaveStrength = 0.2f;
    [SerializeField] float floatWaveSpeed = 3.14f;
 
    [HideInInspector] public bool hasBeenGrabbed = false; 

     /// <summary>Invoked when the card is first grabbed.</summary>
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

    /// <summary>
    /// Stops floating and triggers <see cref="onGrab"/>.
    /// Call this from the XRGrabInteractable “First Select Entered” event.
    /// </summary>
    public void StopFloating()
    {
        onGrab?.Invoke();
    }
}
