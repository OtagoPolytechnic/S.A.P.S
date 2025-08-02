using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
/// <summary>
/// This Class is for Shooting a Grapple Gun
/// notes:
/// current bug where user can stay up in mid air if they stop moving.
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
        playerTransform = Camera.main.transform.root; 

        // Get right hand XR device
        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if (rightHandDevices.Count > 0)
            rightHand = rightHandDevices[0];

        if (ropeRenderer != null)
            ropeRenderer.enabled = false;
    }

    void Update()
    {
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
            Vector3 direction = (grapplePoint - playerTransform.position).normalized;
            playerTransform.position += direction * pullSpeed * Time.deltaTime;

            //float distanceToPoint = Vector3.Distance(playerTransform.position, grapplePoint);
            //if (distanceToPoint < 1f)
            //{
            //    StopGrapple();
            //}

            if (ropeRenderer != null)
            {
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, grapplePoint);
            }
        }
    }
    /// <summary>
    /// shoots a ray cast and records it's location for vector manipulation, also renders a line
    /// </summary>
    void FireGrapple()
    {
        
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            if (ropeRenderer != null)
            {
                ropeRenderer.enabled = true;
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, grapplePoint);
            }
        }
    }




    void StopGrapple()
    {
        isGrappling = false;
        if (ropeRenderer != null)
            ropeRenderer.enabled = false;
    }
}
