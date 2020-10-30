using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public IEnumerator SetName(string name)
    {
        Name.text = name;
        yield return NameBoxHider.ShowAsync();
    }

    public IEnumerator HideName() => NameBoxHider.HideAsync();

    public IEnumerator SetMessage(string message)
    {
        yield return TextRevealer.Off(TextRevealDuration);
        Message.text = message;
        yield return TextRevealer.On(TextRevealDuration);
    }

    [SerializeField]
    GameObject NameBox;
    CanvasGroupHider NameBoxHider;
    Text Name;

    [SerializeField]
    Text Message;

    [SerializeField]
    float TextRevealDuration;

    OnOffAnimator TextRevealer;

    public IEnumerator ShowAsync() => Hider.ShowAsync();
    public IEnumerator HideAsync() => Hider.HideAsync();
    CanvasGroupHider Hider;

    void Awake()
    {
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
        color.a = Mathf.Lerp(0.0f, 1.0f, TextRevealer.Percent);
        Message.color = color;
    }
}
