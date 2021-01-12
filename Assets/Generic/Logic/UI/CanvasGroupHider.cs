using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CanvasGroupHider : MonoBehaviour
{
    public void Show() => ShowAsync(0.0f);
    public void Hide() => HideAsync(0.0f);

    public IEnumerator ShowAsync(float duration) => Animator.On(duration);
    public IEnumerator HideAsync(float duration) => Animator.Off(duration);

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
            CanvasGroup.alpha = Animator.Percent;
    }
}
