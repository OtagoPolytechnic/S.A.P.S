using System.Collections.Generic;
using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// Allocates valid points for crowd-type NPCs to occupy.
/// </summary>
public class CrowdPointAllocator : MonoBehaviour
{
    public List<CrowdPoint> points = new();
    
    /// <summary>
    /// Returns the first available standing point for a crowd NPC.
    /// </summary>
    /// <param name="pointOwner">The NPC that will own the standing point.</param>
    /// <returns>
    /// A tuple containing:
    /// - The index of the standing point in <c>points</c>.
    /// - The <c>Transform</c> of the standing point.
    /// Returns (-1, null) if no valid point exists.
    /// </returns>
    public (int, Transform) ReceiveStandingPoint(GameObject pointOwner)
    {
        foreach (CrowdPoint point in points)
        {
            if (point.isTaken)
            {
                continue;
            }
            else
            {
                point.isTaken = true;
                point.owner = pointOwner;
                return (points.IndexOf(point), point.gameObject.transform);
            }

        }
        //no crowds have a free spot
        return (-1, null);

    }
    
    /// <summary>
    /// Returns available standing points for a group of followers.
    /// </summary>
    /// <param name="followers">The list of followers in the group.</param>
    /// <returns>
    /// A tuple containing:
    /// - The index of the first allocated slot.
    /// - A list of <c>Transform</c> positions for the group.
    /// Returns (-1, null) if no valid points exist.
    /// </returns>
    public (int, List<Transform>) ReceiveStandingPointsForGroup(List<Follower> followers)
    {
        int availableSlots = 0;
        List<Transform> slots = new();
        foreach (CrowdPoint point in points)
        {
            if (point.isTaken)
            {
                continue;
            }
            else
            {
                availableSlots++;
                slots.Add(point.gameObject.transform);
            }
            if (availableSlots > followers.Count)
            {
                foreach (Transform slot in slots)
                {
                    slot.GetComponent<CrowdPoint>().isTaken = true;
                }
                return (slots.IndexOf(slots[0]), slots);
            }
        }
        return (-1, null);
    }
}
