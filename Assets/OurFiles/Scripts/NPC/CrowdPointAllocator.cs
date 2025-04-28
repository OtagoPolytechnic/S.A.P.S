using System.Collections.Generic;
using UnityEngine;
//Written by Rohan Anakin

/// <summary>
/// Allocates valid points for crowd type NPCs to find and take ownership over
/// </summary>
public class CrowdPointAllocator : MonoBehaviour
{
    public List<CrowdPoint> points = new();
    /// <summary>
    /// Returns a valid spot to stand in a crowd if there is one
    /// </summary>
    /// <returns>The integer value and the transform of the spot to stand</returns>
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
    /// Returns valid spots for each follower in a group
    /// </summary>
    /// <param name="followers">The followers in the group</param>
    /// <returns>The integer for the first spot to stand for the leader and a list of transforms for each spot</returns>
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
