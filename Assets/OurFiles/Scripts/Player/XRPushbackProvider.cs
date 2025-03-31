using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

// base 'written' by Joshii
// lots of code yoinked from ContinuousMoveProvider
// head collision detection algorithm discovered from this tutorial:
//   https://www.youtube.com/watch?v=FVPnp3fTGnw
public class XRPushbackProvider : LocomotionProvider
{
    [SerializeField] private float detectionDistance = 0.2f;
    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float pushbackStrength = 1;
    private List<RaycastHit> detectedHits;

    public XROriginMovement Transformation { get; set; } = new XROriginMovement();

    private CharacterController m_CharacterController;
    private bool m_AttemptedGetCharacterController;

    void Update()
    {
        if (mediator.xrOrigin != null ? mediator.xrOrigin.Origin : null == null)
        {
            return;
        }

        FindCharacterController();

        detectedHits = DetectCollision(playerCamera, detectionDistance, detectionLayers);
        Vector3 direction = CalculatePushbackDirection();
        Vector3 motion = pushbackStrength * Time.deltaTime * direction;

        TryStartLocomotionImmediately();
        if (locomotionState == LocomotionState.Moving)
        {
            Transformation.motion = motion;
            TryQueueTransformation(Transformation);
        }
    }

    private void FindCharacterController()
    {
        GameObject gameObject = mediator.xrOrigin != null ? mediator.xrOrigin.Origin : null;
        if (!(gameObject == null) && m_CharacterController == null && !m_AttemptedGetCharacterController)
        {
            if (!gameObject.TryGetComponent<CharacterController>(out m_CharacterController) && gameObject != mediator.xrOrigin.gameObject)
            {
                mediator.xrOrigin.TryGetComponent<CharacterController>(out m_CharacterController);
            }

            m_AttemptedGetCharacterController = true;
        }
    }

    private List<RaycastHit> DetectCollision(Transform origin, float distance, LayerMask mask)
    {
        List<RaycastHit> newDetectedHits = new();

        List<Vector3> directions = new() { origin.forward, origin.right, -origin.right };
        
        RaycastHit hit;
        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(origin.position, direction, out hit, distance, mask))
            {
                newDetectedHits.Add(hit);
            }
        }

        return newDetectedHits;
    }

    private Vector3 CalculatePushbackDirection()
    {
        Vector3 combinedNormal = Vector3.zero;
        foreach (RaycastHit hit in detectedHits)
        {
            combinedNormal += hit.normal;
        }
        combinedNormal.y = 0;
        return combinedNormal.normalized;
    }
}
