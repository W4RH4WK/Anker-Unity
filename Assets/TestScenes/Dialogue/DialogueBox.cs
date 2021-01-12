using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public IEnumerator SetName(string name)
    {
        Name.text = name;
        yield return NameBoxHider.ShowAsync(System.AnimationDuration);
    }

    public IEnumerator HideName() => NameBoxHider.HideAsync(System.AnimationDuration);

    public IEnumerator SetMessage(string message)
    {
        yield return TextRevealer.Off(System.AnimationDuration);
        Message.text = message;
        yield return TextRevealer.On(System.AnimationDuration);
    }

    [SerializeField]
    GameObject NameBox;
    CanvasGroupHider NameBoxHider;
    Text Name;

    [SerializeField]
    Text Message;

    DialogueSystem System;

    OnOffAnimator TextRevealer;

    public IEnumerator ShowAsync() => Hider.ShowAsync(System.AnimationDuration);
    public IEnumerator HideAsync() => Hider.HideAsync(System.AnimationDuration);
    CanvasGroupHider Hider;

    void Awake()
    {
        System = GetComponentInParent<DialogueSystem>();
        Assert.IsNotNull(System);

        TextRevealer = new OnOffAnimator();

        Assert.IsNotNull(NameBox);
        NameBoxHider = NameBox.GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(NameBoxHider);
        Name = NameBox.GetComponentInChildren<Text>();
        Assert.IsNotNull(Name);

        Assert.IsNotNull(Message);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }

    void Update()
    {
        var color = Message.color;
        color.a = TextRevealer.Percent;
        Message.color = color;
    }
}
