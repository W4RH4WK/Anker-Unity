using UnityEngine;
using System.Collections.Generic;

public class AnchorRadius : MonoBehaviour
{
    public IList<AnchorPoint> AnchorPointsInRange => _AnchorPointsInRange.AsReadOnly();
    List<AnchorPoint> _AnchorPointsInRange = new List<AnchorPoint>();

    void OnTriggerEnter2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (anchorPoint)
            _AnchorPointsInRange.Add(anchorPoint);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var anchorPoint = collider.GetComponent<AnchorPoint>();
        if (anchorPoint)
            _AnchorPointsInRange.Remove(anchorPoint);
    }
}
