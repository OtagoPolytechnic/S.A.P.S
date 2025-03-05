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

    private PauseState pauseState = PauseState.Play;

    public UnityEvent<PauseState> PauseChange = new UnityEvent<PauseState>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void PauseGame()
    {
        if (pauseState == PauseState.Play)
        {
            pauseState = PauseState.Paused;
            PauseChange?.Invoke(pauseState);
        }
    }

    public void ResumeGame()
    {
        if (pauseState == PauseState.Paused)
        {
            pauseState = PauseState.Play;
            PauseChange?.Invoke(pauseState);
        }
    }

    public void TogglePauseState()
    {
        if (pauseState == PauseState.Play) PauseGame();
        else ResumeGame();
    }
}
