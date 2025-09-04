using UnityEngine;
using UnityEngine.Events;

public class PlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) onPlayerEnter?.Invoke();
    }
}
