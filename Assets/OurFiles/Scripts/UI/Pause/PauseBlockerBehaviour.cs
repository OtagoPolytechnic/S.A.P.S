using UnityEngine;

/// <summary>
/// Enables a set of walls around the player when the game is paused,
/// preventing unwanted movement or interaction during pause.
/// </summary>
public class PauseBlockerBehaviour : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject wallsParent;

    private void Start()
    {
        PauseManager.Instance.PauseChange.AddListener(OnPauseChange);

        OnPauseChange(PauseManager.Instance.State);
    }

    private void OnPauseChange(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            wallsParent.SetActive(true);
            wallsParent.transform.position = player.position;
        }
        else
        {
            wallsParent.SetActive(false);
        }
    }
}
