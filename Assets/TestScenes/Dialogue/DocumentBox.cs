using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DocumentBox : MonoBehaviour
{
    public IEnumerator SetText(string title, string text, char separator = '|')
    {
        DocumentTextParts.Clear();
        Spinner.Hide();

        DocumentTitle.text = title;
        yield return this.Par(DocumentTitleAnimator.On(System.DocumentTextRevealDuration), System.WaitForContinue());

        var parts = text.Split(separator);

        for (var i = 0; i < parts.Count(); i++)
        {
            DocumentTextParts.Add(parts[i]);
            DocumentTextPartTimer.Start(System.DocumentTextRevealDuration);

            if (i == parts.Count() - 1)
                yield return this.Par(DocumentTextPartTimer.Wait(), Spinner.ShowAsync(System.AnimationDuration));

            yield return System.WaitForContinue();
        }

        DocumentTitleAnimator.Off();
        DocumentTitle.text = "";
        DocumentTextParts.Clear();
    }

    [SerializeField]
    Text DocumentTitle;
    OnOffAnimator DocumentTitleAnimator = new OnOffAnimator();

    [SerializeField]
    Text DocumentText;
    IList<string> DocumentTextParts = new List<string>();
    Timer DocumentTextPartTimer = new Timer();

    [SerializeField]
    CanvasGroupHider Spinner;

    DialogueSystem System;

    public IEnumerator ShowAsync() => Hider.ShowAsync(System.AnimationDuration);
    public IEnumerator HideAsync() => Hider.HideAsync(System.AnimationDuration);
    CanvasGroupHider Hider;

    void Awake()
    {
        Assert.IsNotNull(DocumentTitle);
        Assert.IsNotNull(DocumentText);
        Assert.IsNotNull(Spinner);

        System = GetComponentInParent<DialogueSystem>();
        Assert.IsNotNull(System);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }

    void Update()
    {
        {
            var titleColor = DocumentTitle.color;
            titleColor.a = DocumentTitleAnimator.Percent;
            DocumentTitle.color = titleColor;
        }

        var color = DocumentText.color;

        color.a = System.DocumentTextFadedOutAlpha;
        DocumentText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        for (var i = 0; i < DocumentTextParts.Count - 2; i++)
            DocumentText.text += DocumentTextParts[i];
        DocumentText.text += "</color>";

        color.a = Mathf.SmoothStep(1.0f, System.DocumentTextFadedOutAlpha, DocumentTextPartTimer.Percent);
        DocumentText.text +=
            $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{DocumentTextParts.ElementAtOrDefault(DocumentTextParts.Count - 2)}</color>";

        color.a = Mathf.SmoothStep(0.0f, 1.0f, DocumentTextPartTimer.Percent);
        DocumentText.text +=
            $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{DocumentTextParts.LastOrDefault()}</color>";
    }
}
