using UnityEngine;

//Written by Rohan Anakin
/// <summary>
/// Opens the elevator doors on trigger enter.
/// </summary>
public class ElevatorOpener : MonoBehaviour
{
    [SerializeField]
    Elevator elevator;
    bool hasEntered;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (!hasEntered)
            {
                elevator.OpenDoors();
                hasEntered = true;
            }
        }
    }
}
