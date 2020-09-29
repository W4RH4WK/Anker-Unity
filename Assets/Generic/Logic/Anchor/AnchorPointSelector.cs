using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnchorPointSelector : MonoBehaviour
{
    public AnchorPoint GetAnchorPoint(Vector2 input, Orientation orientation)
    {
        if (input.magnitude == 0.0f)
            input.x = orientation.ToFactor();

        Func<AnchorPoint, float> angleToPoint = p => Vector2.Angle(p.transform.position - transform.position, input);

        return AnchorPointsInRange.Where(p => angleToPoint(p) < 90.0f).OrderBy(angleToPoint).ElementAtOrDefault(0);
    }

    IList<AnchorPoint> AnchorPointsInRange = new List<AnchorPoint>();

    void OnTriggerEnter2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (anchorPoint)
            AnchorPointsInRange.Add(anchorPoint);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (anchorPoint)
            AnchorPointsInRange.Remove(anchorPoint);
    }
}
