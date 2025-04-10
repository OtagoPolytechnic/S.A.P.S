using System.Collections.Generic;
using UnityEngine;

public class CrowdPointAllocator : MonoBehaviour
{
    public List<GameObject> points = new();

    public (int, Transform) ReceiveStandingPoint(GameObject pointOwner)
    {
        foreach (GameObject p in points)
        {
            CrowdPoint point = p.GetComponent<CrowdPoint>();
            if (point.isTaken)
            {
                continue;
            }
            else
            {
                point.isTaken = true;
                point.owner = pointOwner;
                return (points.IndexOf(p), p.transform);
            }

        }
        //no crowds have a free spot
        return (-1, null);
        
    }
}
