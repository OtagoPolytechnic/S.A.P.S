using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

// base written by joshii

/// <summary>
/// Provides configurable alignment for an XR rig camera
/// </summary>
public class XRAlignmentProvider : LocomotionProvider
{
    private XRBodyTransformer transformer;

    void Start()
    {
        // the locomotion object on the XR rig holds both the locomotion mediator and body transformer components
        // https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/locomotion.html#xr-body-transformer
        transformer = mediator.gameObject.GetComponent<XRBodyTransformer>();
        AlignCamera(Vector3.forward);
    }

    /// <summary>
    /// Rotates the XR rig camera to align with the direction across the XZ plane
    /// https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.XRCameraForwardXZAlignment.html
    /// </summary>
    /// <param name="direction">The direction to align to on the XZ plane (Y is ignored)</param>
    void AlignCamera(Vector3 direction)
    {
        TryStartLocomotionImmediately();
        XRCameraForwardXZAlignment alignment = new()
        {
            targetDirection = direction
        };
        TryQueueTransformation(alignment);
        Debug.Log("Recentered??");
    }
}