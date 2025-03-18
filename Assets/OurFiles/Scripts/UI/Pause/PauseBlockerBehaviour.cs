using UnityEngine;

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
