using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialoguePortrait : MonoBehaviour
{
    public IEnumerator Set(Sprite sprite)
    {
        if (Image.sprite != sprite)
            yield return HideAsync();

        Image.sprite = sprite;
        yield return ShowAsync();
    }

    public IEnumerator ShowAsync() => Hider.ShowAsync();
    public IEnumerator HideAsync() => Hider.HideAsync();
    CanvasGroupHider Hider;

    public IEnumerator RaiseAsync() => RaiseAnimator.On(RaiseDuration);
    public IEnumerator LowerAsync() => RaiseAnimator.Off(RaiseDuration);

    [SerializeField]
    float RaiseDuration;

    [SerializeField]
    Color RaiseTint;

    [SerializeField]
    Color LowerTint;

    OnOffAnimator RaiseAnimator;

    Image Image;

    void Awake()
    {
        RaiseAnimator = new OnOffAnimator();

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);

        Image = GetComponentInChildren<Image>();
        Assert.IsNotNull(Image);
    }

    void Update()
    {
        Image.color = Color.Lerp(LowerTint, RaiseTint, RaiseAnimator.Percent);
    }
}
