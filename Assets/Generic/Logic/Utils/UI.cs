using UnityEngine;
using UnityEngine.Assertions;

static class UIUtils
{
    /// <summary>
    /// Calculate the natural off-screen position for boundary anchored UI
    /// elements. Only simple anchor points are supported.
    /// </summary>
    public static Vector2 OffscreenPosition(RectTransform element, float margin = 50.0f)
    {
        var hideDirection = 2.0f * element.anchorMin - Vector2.one;
        return hideDirection * (element.rect.size + element.anchoredPosition.Abs() + margin * Vector2.one) +
               element.anchoredPosition;
    }
}
