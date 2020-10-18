using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

static class UIUtils
{
    /// <summary>
    /// Calculate the natural off-screen position for boundary anchored UI
    /// elements.
    /// </summary>
    public static Vector2 OffscreenPosition(RectTransform element, float margin = 50.0f)
    {
        // Only supporting simple anchor points.
        Assert.IsTrue(element.anchorMin == element.anchorMax);

        var hideDirection = 2.0f * element.anchorMin - Vector2.one;
        return hideDirection * (element.rect.size + element.anchoredPosition.Abs() + margin * Vector2.one) +
               element.anchoredPosition;
    }
}

static class RectTransformExtensions
{
    public static IEnumerator MoveTowardsAsync(this RectTransform rect, Vector2 target, float maxDistanceDelta)
    {
        while (rect.anchoredPosition != target)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, target, maxDistanceDelta);
            yield return null;
        }
    }
}
