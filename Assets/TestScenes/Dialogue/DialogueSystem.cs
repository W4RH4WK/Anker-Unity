using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class DialogueSystem : MonoBehaviour, ISubmitHandler
{
    public IEnumerator Say(string text)
    {
        Box.SetMessage(text);

        EventSystem.current.SetSelectedGameObject(gameObject);

        yield return Box.ShowAsync();

        Continue = false;
        while (!Continue)
            yield return null;
    }

    public IEnumerator Say(string name, string text)
    {
        Box.SetName(name);
        return Say(text);
    }

    public IEnumerator Say(DialogueCharacter character, string text) => this.Par(SetPortraitLeft(character.Image),
                                                                                 Say(character.name, text));

    public IEnumerator SayRight(DialogueCharacter character, string text) => this.Par(SetPortraitRight(character.Image),
                                                                                      Say(character.name, text));

    public IEnumerator Tell(string text)
    {
        Box.HideName();
        return Say(text);
    }

    public IEnumerator SetPortraitLeft(Sprite image) => PortraitLeft.Set(image);
    public IEnumerator SetPortraitRight(Sprite image) => PortraitRight.Set(image);

    public IEnumerator Hide() => this.Par(Box.HideAsync(), PortraitLeft.HideAsync(), PortraitRight.HideAsync());

    //////////////////////////////////////////////////////////////////////////

    public void OnSubmit(BaseEventData eventData) => Continue = true;
    bool Continue;

    //////////////////////////////////////////////////////////////////////////

    [SerializeField]
    DialogueBox Box;

    [SerializeField]
    DialoguePortrait PortraitLeft;

    [SerializeField]
    DialoguePortrait PortraitRight;

    void Awake()
    {
        Assert.IsNotNull(Box);
        Assert.IsNotNull(PortraitLeft);
        Assert.IsNotNull(PortraitRight);
    }
}
