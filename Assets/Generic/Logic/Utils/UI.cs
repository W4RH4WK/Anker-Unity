using System.Collections;
using UnityEngine;

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
