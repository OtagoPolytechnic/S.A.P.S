using UnityEngine;
using UnityEngine.Events;

public class PlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) onPlayerEnter?.Invoke();
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) onPlayerExit?.Invoke();
    }
}
