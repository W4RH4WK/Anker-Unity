using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public void SetName(string name)
    {
        NameBox.SetActive(true);
        Name.text = name;
    }

    public void HideName() => NameBox.SetActive(false);

    [SerializeField]
    GameObject NameBox;
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
        Name = GetComponentInChildren<Text>(NameBox);
        Assert.IsNotNull(Name);

        Assert.IsNotNull(Message);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }
}
