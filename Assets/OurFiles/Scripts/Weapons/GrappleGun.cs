using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
/// <summary>
/// Handles firing a grappling hook onto a surface marked with the specified grappleLayer.
/// Pulls the player toward the grapple point and renders a rope.
/// 
/// Notes:
/// - Bug: Player can remain suspended mid-air if they stop moving.
/// 
/// Created by: Mike McPherson
/// Date: 2025-08-04
/// </summary>
public class GrappleGun : MonoBehaviour
{
  
    public float maxGrappleDistance = 30f;
    public float pullSpeed = 10f;
    public LayerMask grappleLayer;
    public LineRenderer ropeRenderer;

    private Transform playerTransform;
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private InputDevice rightHand;

    void Start()
    {
        // Get the player's root transform (headset base)
        playerTransform = Camera.main.transform.root;

        // Retrieve the XR device for the right hand
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
            rightHand = rightHandDevices[0];
        // Disable the rope line renderer at the start
        if (ropeRenderer != null)
            ropeRenderer.enabled = false;
    }

    void Update()
    {
        // Check if the right hand device is valid and get trigger input
        if (rightHand.isValid && rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed)) 
        {
            if (triggerPressed && !isGrappling)
            {
                FireGrapple();
            }
            else if (!triggerPressed && isGrappling)
            {
                StopGrapple();
            }
        }

        if (isGrappling)
        {
            // Move the player toward the grapple point
            Vector3 direction = (grapplePoint - playerTransform.position).normalized; 
            playerTransform.position += direction * pullSpeed * Time.deltaTime;
            // Update rope positions every frame
            if (ropeRenderer != null)
            {
                
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, grapplePoint); 
            }
        }
    }

    // <summary>
    // Fires a ray forward to detect a valid grapple surface and sets the target point.
    // Also enables and positions the rope renderer.
    // </summary>
    void FireGrapple()
    {
        
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point; //the hit object stores information such as location etc.
            isGrappling = true;

            if (ropeRenderer != null)
            {
                //create the point for the grapple line animation from a to b 
                ropeRenderer.enabled = true;
                ropeRenderer.SetPosition(0, transform.position); 
                ropeRenderer.SetPosition(1, grapplePoint);
            }
        }
    }
    // <summary>
    // logic for when the player has stopped grappling 
    // </summary>
    void StopGrapple()
    {
        isGrappling = false;
        if (ropeRenderer != null)
            ropeRenderer.enabled = false; //remove the line when trigger is released 
    }
}
