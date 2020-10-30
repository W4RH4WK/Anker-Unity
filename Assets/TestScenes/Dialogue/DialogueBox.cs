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

    [SerializeField]
    GameObject NameBox;
    CanvasGroupHider NameBoxHider;
    Text Name;

    public IEnumerator SetMessage(string message)
    {
        var startTime = Time.time;
        var endTime = startTime + message.Length / TextSpeed;

        while (true)
        {
            var percent = Mathf.InverseLerp(startTime, endTime, Time.time);
            var numLetters = Mathf.Lerp(0, message.Length, percent);

            Message.text = message.Substring(0, (int)numLetters);

            if (numLetters != message.Length)
                yield return null;
            else
                yield break;
        }
    }

    [SerializeField]
    Text Message;

    [SerializeField]
    float TextSpeed;

    public IEnumerator ShowAsync() => Hider.ShowAsync();
    public IEnumerator HideAsync() => Hider.HideAsync();
    CanvasGroupHider Hider;

    void Awake()
    {
        Assert.IsNotNull(NameBox);
        NameBoxHider = NameBox.GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(NameBoxHider);
        Name = NameBox.GetComponentInChildren<Text>();
        Assert.IsNotNull(Name);

        Assert.IsNotNull(Message);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }
}
