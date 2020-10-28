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

    public void SetMessage(string message) => Message.text = message;

    [SerializeField]
    Text Message;

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
