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

/// <summary>
/// Helper class to hide a given UI element off-screen.
/// </summary>
class OffscreenHider
{
    public OffscreenHider(RectTransform element)
    {
        Assert.IsNotNull(element);

        Element = element;
        ShowPosition = Element.anchoredPosition;
        HidePosition = UIUtils.OffscreenPosition(Element);

        Hide();
    }

    public void Show() => Element.anchoredPosition = ShowPosition;
    public void Hide() => Element.anchoredPosition = HidePosition;
    public IEnumerator Show(float speed) => Element.MoveTowardsAsync(ShowPosition, speed);
    public IEnumerator Hide(float speed) => Element.MoveTowardsAsync(HidePosition, speed);

    RectTransform Element;

    Vector2 ShowPosition;
    Vector2 HidePosition;
}
