using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DocumentBox : MonoBehaviour
{
    public IEnumerator Set(string title, string body, char separator = '|')
    {
        BodyParts.Clear();
        Spinner.Hide();

        Title.text = title;
        yield return this.Par(TitleAnimator.On(System.TextRevealDuration),
                              FindObjectOfType<Filament>().WaitForContinue()); // FIXME

        var parts = body.Split(separator);

        for (var i = 0; i < parts.Count(); i++)
        {
            BodyParts.Add(parts[i]);
            BodyPartTimer.Start(System.TextRevealDuration);

            if (i == parts.Count() - 1)
                yield return this.Par(BodyPartTimer.Wait(), Spinner.ShowAsync(System.AnimationDuration));

            yield return FindObjectOfType<Filament>().WaitForContinue(); // FIXME
        }

        TitleAnimator.Off();
        Title.text = "";
        BodyParts.Clear();
    }

    [SerializeField]
    Text Title;
    OnOffAnimator TitleAnimator = new OnOffAnimator();

    [SerializeField]
    Text Body;
    IList<string> BodyParts = new List<string>();
    Timer BodyPartTimer = new Timer();

    [SerializeField]
    CanvasGroupHider Spinner;

    DocumentSystem System;

    public IEnumerator ShowAsync() => Hider.ShowAsync(System.AnimationDuration);
    public IEnumerator HideAsync() => Hider.HideAsync(System.AnimationDuration);
    CanvasGroupHider Hider;

    void Awake()
    {
        Assert.IsNotNull(Title);
        Assert.IsNotNull(Body);
        Assert.IsNotNull(Spinner);

        System = GetComponentInParent<DocumentSystem>();
        Assert.IsNotNull(System);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }

    void Update()
    {
        {
            var titleColor = Title.color;
            titleColor.a = TitleAnimator.Percent;
            Title.color = titleColor;
        }

        var color = Body.color;

        color.a = System.TextFadedOutAlpha;
        Body.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        for (var i = 0; i < BodyParts.Count - 2; i++)
            Body.text += BodyParts[i];
        Body.text += "</color>";

        color.a = Mathf.SmoothStep(1.0f, System.TextFadedOutAlpha, BodyPartTimer.Percent);
        Body.text +=
            $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{BodyParts.ElementAtOrDefault(BodyParts.Count - 2)}</color>";

        color.a = Mathf.SmoothStep(0.0f, 1.0f, BodyPartTimer.Percent);
        Body.text += $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{BodyParts.LastOrDefault()}</color>";
    }
}
