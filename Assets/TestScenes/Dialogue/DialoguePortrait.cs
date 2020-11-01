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

    public IEnumerator ShowAsync() => Hider.ShowAsync(System.AnimationDuration);
    public IEnumerator HideAsync() => Hider.HideAsync(System.AnimationDuration);
    CanvasGroupHider Hider;

    public IEnumerator RaiseAsync() => RaiseAnimator.On(System.AnimationDuration);
    public IEnumerator LowerAsync() => RaiseAnimator.Off(System.AnimationDuration);

    [SerializeField]
    Color RaiseTint;

    [SerializeField]
    Color LowerTint;

    DialogueSystem System;

    OnOffAnimator RaiseAnimator;

    Image Image;

    void Awake()
    {
        System = GetComponentInParent<DialogueSystem>();
        Assert.IsNotNull(System);

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
