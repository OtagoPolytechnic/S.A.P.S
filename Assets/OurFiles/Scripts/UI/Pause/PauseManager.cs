using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public enum PauseState
{
    Paused,
    Play
}

/// <summary>
/// Manages whether the game is in paused or play mode, 
/// raising events when the state changes.
/// </summary>
public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private InputActionManager inputActionManager;

    public UnityEvent<PauseState> PauseChange = new UnityEvent<PauseState>();

    private PauseState state = PauseState.Play;
    public PauseState State
    {
        get => state; set
        {
            state = value;

            if (state == PauseState.Play)
            {
                PauseChange?.Invoke(state);
            }
            else
            {
                PauseChange?.Invoke(state);
            }
        }
    }

    private void Start()
    {
        InputActionAsset asset =  inputActionManager.actionAssets[0];
        InputActionMap actionMap = asset.FindActionMap("XRI Left Interaction");
        //InputAction action = actionMap.FindAction("Menu"); // using steam vr bindings for menu button
        InputAction action = actionMap.FindAction("Menu");
        action.performed += context => TogglePauseState();
    }

    public void TogglePauseState()
    {
        if (state == PauseState.Play) State = PauseState.Paused;
        else State = PauseState.Play;
    }
}
