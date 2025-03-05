using UnityEngine;

public class PauseBlockerBehaviour : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject wallsParent;

    private void Start()
    {
        PauseManager.Instance.PauseChange.AddListener(OnPauseChange);
    }

    private void OnPauseChange(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            wallsParent.SetActive(true);
        }
        else
        {
            wallsParent.SetActive(false);
        }
    }
}
