using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DocumentBox : MonoBehaviour
{
    public IEnumerator SetText(string text, char separator = '|')
    {
        DocumentTextParts.Clear();

        var parts = text.Split(separator);

        foreach (var part in parts)
        {
            DocumentTextParts.Add(part);
            DocumentTextPartTimer.Start(System.DocumentTextRevealDuration);
            yield return System.WaitForContinue();
        }
    }

    Text DocumentText;
    IList<string> DocumentTextParts = new List<string>();
    Timer DocumentTextPartTimer = new Timer();

    DialogueSystem System;

    public IEnumerator ShowAsync() => Hider.ShowAsync(System.AnimationDuration);
    public IEnumerator HideAsync() => Hider.HideAsync(System.AnimationDuration);
    CanvasGroupHider Hider;

    void Awake()
    {
        System = GetComponentInParent<DialogueSystem>();
        Assert.IsNotNull(System);

        DocumentText = GetComponentInChildren<Text>();
        Assert.IsNotNull(DocumentText);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }

    void Update()
    {
        DocumentText.text = "";

        for (var i = 0; i < DocumentTextParts.Count - 1; i++)
            DocumentText.text += DocumentTextParts[i];

        var color = DocumentText.color;
        color.a = Mathf.Lerp(0.0f, 1.0f, DocumentTextPartTimer.Percent);

        DocumentText.text +=
            $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{DocumentTextParts.LastOrDefault()}</color>";
    }
}
