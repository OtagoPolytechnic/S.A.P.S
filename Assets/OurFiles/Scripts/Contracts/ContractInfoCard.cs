using System;
using UnityEngine;
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

    [Header("Grab Interaction")]
    [SerializeField] GameObject leftController;
    [SerializeField] GameObject[] leftControllerMeshes;
    [SerializeField] Vector3 inHandTransformOffset = new(0, 0.03f, -0.03f); //example values
    [SerializeField] Quaternion inHandRotationOffset = Quaternion.Euler(44.5f, 0, 0);

    private float baseYPos;
    private bool isVisible;

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

        //turn off interaction (ignore the obsolete warning)
        GetComponent<XRGrabInteractable>().enabled = false;
        GetComponent<XRGeneralGrabTransformer>().enabled = false;
        GetComponentInChildren<XRInteractableAffordanceStateProvider>().gameObject.SetActive(false);

        //move into left hand
        transform.SetParent(leftController.transform);
        transform.position += inHandTransformOffset;
        transform.rotation = inHandRotationOffset;

        ToggleVision();
        
    }

    private void ToggleVision() 
    {
        isVisible = !isVisible;
        foreach (GameObject mesh in leftControllerMeshes)
        {
            mesh.SetActive(!isVisible);
        }
    }
}
