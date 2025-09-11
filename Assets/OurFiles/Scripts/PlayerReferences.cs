using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerReferences : Singleton<PlayerReferences>
{
    [SerializeField] SnapTurnProvider snapTurn;
    public SnapTurnProvider SnapTurn
    {
        get => snapTurn;
        set => snapTurn = value;
    }
    [SerializeField] ContinuousTurnProvider continuousTurn;
    public ContinuousTurnProvider SmoothTurn
    {
        get => continuousTurn;
        set => continuousTurn = value;
    }

    [SerializeField] ControllerInputActionManager rightControllerInput;
    public ControllerInputActionManager RightControllerInput
    {
        get => rightControllerInput;
        set => rightControllerInput = value;
    }
}
