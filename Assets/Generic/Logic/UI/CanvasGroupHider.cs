using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CanvasGroupHider : MonoBehaviour
{
    public IEnumerator ShowAsync() => Animator.On(Duration);
    public IEnumerator HideAsync() => Animator.Off(Duration);

    [SerializeField]
    float Duration;

    [SerializeField]
    bool Position;

    [SerializeField]
    bool Transparency;

    Vector2 ShowPosition;
    Vector2 HidePosition;

    OnOffAnimator Animator;

    RectTransform RectTransform;
    CanvasGroup CanvasGroup;

    void Awake()
    {
        Animator = new OnOffAnimator();

        RectTransform = GetComponent<RectTransform>();
        Assert.IsNotNull(RectTransform);

        CanvasGroup = GetComponent<CanvasGroup>();
        Assert.IsNotNull(CanvasGroup);

        ShowPosition = RectTransform.anchoredPosition;
        HidePosition = UIUtils.OffscreenPosition(RectTransform);
    }

    void Update()
    {
        if (Position)
            RectTransform.anchoredPosition = Vector2.Lerp(HidePosition, ShowPosition, Animator.Percent);

        if (Transparency)
            CanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, Animator.Percent);
    }
}
