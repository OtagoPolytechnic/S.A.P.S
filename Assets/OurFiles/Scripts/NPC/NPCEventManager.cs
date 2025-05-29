using UnityEngine;
using UnityEngine.Events;

// Base written by Jenna

public class NPCEventManager : Singleton<NPCEventManager>
{
    [HideInInspector]
    public UnityEvent<GameObject> onPanic = new(); //when an NPC gets paniced it should call this event and give its own GameObject
}
