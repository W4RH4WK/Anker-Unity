using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CanvasGroupHider : MonoBehaviour
{
    public void Show() => Hiding = false;
    public void Hide() => Hiding = true;

    public IEnumerator ShowAsync()
    {
        if (!Hiding)
            yield break;

        Show();
        DurationLeft = Duration;

        while (DurationLeft > 0.0f)
            yield return null;
    }

    public IEnumerator HideAsync()
    {
        if (Hiding)
            yield break;

        Hide();
        DurationLeft = Duration;

        while (DurationLeft > 0.0f)
            yield return null;
    }

    public float Duration;
    float DurationLeft;

    bool Hiding;

    Vector2 ShowPosition;
    Vector2 HidePosition;

    RectTransform RectTransform;
    CanvasGroup CanvasGroup;

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Assert.IsNotNull(RectTransform);

        CanvasGroup = GetComponent<CanvasGroup>();
        Assert.IsNotNull(CanvasGroup);

        ShowPosition = RectTransform.anchoredPosition;
        HidePosition = UIUtils.OffscreenPosition(RectTransform);

        Hide();
    }

    void Update()
    {
        DurationLeft -= Time.deltaTime;

        var percent = Mathf.InverseLerp(Duration, 0.0f, DurationLeft);
        if (Hiding)
            percent = 1 - percent;

        RectTransform.anchoredPosition = Vector2.Lerp(HidePosition, ShowPosition, percent);
        CanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, percent);
    }
}
