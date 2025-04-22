using System.Collections.Generic;
using UnityEngine;

public class CrowdPointAllocator : MonoBehaviour
{
    public List<CrowdPoint> points = new();

    public (int, Transform) ReceiveStandingPoint()
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
                return (points.IndexOf(point), point.gameObject.transform);
            }

        }
        //no crowds have a free spot
        return (-1, null);
        
    }

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
