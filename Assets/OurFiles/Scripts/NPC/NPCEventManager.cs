using UnityEngine;
using UnityEngine.Events;

// Base written by Jenna

/// <summary>
/// Global event hub for NPC-related events, such as panic and player arrest.
/// </summary>
public class NPCEventManager : Singleton<NPCEventManager>
{
    /// <summary>
    /// Invoked when an NPC enters a panic state, passing the NPC's <see cref="GameObject"/>.
    /// </summary>
    [HideInInspector]
    public UnityEvent<GameObject> onPanic = new();
    
    /// <summary>
    /// Invoked when the player is arrested by a guard.
    /// </summary>
    public UnityEvent onPlayerArrested = new();
}
