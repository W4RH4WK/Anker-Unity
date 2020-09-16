using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnchorRadius : MonoBehaviour
{
    public AnchorPoint GetAnchorPoint(Vector2 input, Orientation orientation) => Get(AnchorPointsInRange, input,
                                                                                     orientation);

    public AnchorPoint GetEnemy(Vector2 input, Orientation orientation) => Get(EnemiesInRange, input, orientation);

    AnchorPoint Get(IList<AnchorPoint> points, Vector2 input, Orientation orientation)
    {
        if (input.magnitude == 0.0f)
            input.x = orientation.ToFactor();

        Func<AnchorPoint, float> angleToPoint = p => Vector2.Angle(p.transform.position - transform.position, input);

        return points.Where(p => angleToPoint(p) < 90.0f).OrderBy(angleToPoint).ElementAtOrDefault(0);
    }

    IList<AnchorPoint> AnchorPointsInRange = new List<AnchorPoint>();
    IList<AnchorPoint> EnemiesInRange = new List<AnchorPoint>();

    void OnTriggerEnter2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (!anchorPoint)
            return;

        if (IsEnemy(anchorPoint))
            EnemiesInRange.Add(anchorPoint);
        else
            AnchorPointsInRange.Add(anchorPoint);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (!anchorPoint)
            return;

        if (IsEnemy(anchorPoint))
            EnemiesInRange.Remove(anchorPoint);
        else
            AnchorPointsInRange.Remove(anchorPoint);
    }

    static bool IsEnemy(AnchorPoint p) => p.gameObject.layer == LayerMask.NameToLayer("Enemy");
}
