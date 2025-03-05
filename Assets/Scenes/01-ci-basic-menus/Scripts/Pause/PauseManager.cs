using UnityEngine;
using UnityEngine.Events;

public enum PauseState
{
    Paused,
    Play
}

public class PauseManager : MonoBehaviour
{
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

    public void TogglePauseState()
    {
        if (state == PauseState.Play) State = PauseState.Paused;
        else State = PauseState.Play;
    }
}
