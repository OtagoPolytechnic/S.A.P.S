using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public enum PauseState
{
    Paused,
    Play
}

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputActionManager inputActionManager;

    public static PauseManager Instance { get; private set; }

    public UnityEvent<PauseState> PauseChange = new UnityEvent<PauseState>();

    private PauseState state = PauseState.Play;
    public PauseState State
    {
        get => state; set
        {
            state = value;

            if (state == PauseState.Play)
            {
                state = PauseState.Paused;
                PauseChange?.Invoke(state);
            }
            else
            {
                state = PauseState.Play;
                PauseChange?.Invoke(state);
            }
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InputActionAsset asset =  inputActionManager.actionAssets[0];
        InputActionMap actionMap = asset.FindActionMap("XRI Left Interaction");
        InputAction action = actionMap.FindAction("Menu");
        Debug.Log(actionMap + ", " + action);
        action.performed += context => TogglePauseState();
    }

    public void TogglePauseState()
    {
        Debug.Log("TOGGLING");

        if (state == PauseState.Play) State = PauseState.Paused;
        else State = PauseState.Play;
    }
}
