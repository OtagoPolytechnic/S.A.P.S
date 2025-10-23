using UnityEngine;

//Written by Rohan Anakin

/// <summary>
/// Opens elevator doors when the player enters.
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
