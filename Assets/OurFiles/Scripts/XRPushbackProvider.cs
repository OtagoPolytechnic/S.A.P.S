using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

// lots of code yoinked from ContinuousMoveProvider
public class XRPushbackProvider : LocomotionProvider
{
    public XROriginMovement transformation { get; set; } = new XROriginMovement();

    private CharacterController m_CharacterController;
    private bool m_AttemptedGetCharacterController;

    void Update()
    {
        if (mediator.xrOrigin?.Origin == null)
        {
            return;
        }

        FindCharacterController();
        Vector3 motion = Vector3.zero;

        TryStartLocomotionImmediately();
        if (base.locomotionState == LocomotionState.Moving)
        {
            transformation.motion = motion;
            TryQueueTransformation(transformation);
        }
    }

    private void FindCharacterController()
    {
        GameObject gameObject = base.mediator.xrOrigin?.Origin;
        if (!(gameObject == null) && m_CharacterController == null && !m_AttemptedGetCharacterController)
        {
            if (!gameObject.TryGetComponent<CharacterController>(out m_CharacterController) && gameObject != base.mediator.xrOrigin.gameObject)
            {
                base.mediator.xrOrigin.TryGetComponent<CharacterController>(out m_CharacterController);
            }

            m_AttemptedGetCharacterController = true;
        }
    }
}
