using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// base written by Joshii

/// <summary>
/// Floats in air until grabbed by player
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class ContractInfoCard : MonoBehaviour
{
    [SerializeField] float floatWaveStrength = 0.2f;
    [SerializeField] float floatWaveSpeed = 3.14f;

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
        enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    void OnCollisionEnter(Collision other)
    {
        StopFloating();
    }
}
