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

        yield return this.Par(Box.ShowAsync(), Background.On());

        Continue = false;
        while (!Continue)
            yield return null;
    }

    public IEnumerator Say(string name, string text)
    {
        Box.SetName(name);
        return Say(text);
    }

    public IEnumerator Say(DialogueCharacter character,
                           string text) => this.Par(PortraitLeft.RaiseAsync(), PortraitRight.LowerAsync(),
                                                    SetPortraitLeft(character.Image), Say(character.name, text));

    public IEnumerator SayRight(DialogueCharacter character,
                                string text) => this.Par(PortraitLeft.LowerAsync(), PortraitRight.RaiseAsync(),
                                                         SetPortraitRight(character.Image), Say(character.name, text));

    public IEnumerator Tell(string text)
    {
        Box.HideName();
        yield return this.Par(PortraitLeft.LowerAsync(), PortraitRight.LowerAsync());
        yield return Say(text);
    }

    IEnumerator SetPortraitLeft(Sprite image) => PortraitLeft.Set(image);
    IEnumerator SetPortraitRight(Sprite image) => PortraitRight.Set(image);

    public IEnumerator Hide() => this.Par(Box.HideAsync(), PortraitLeft.HideAsync(), PortraitRight.HideAsync(),
                                          Background.Off());

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

    [SerializeField]
    DialogueBackground Background;

    void Awake()
    {
        Assert.IsNotNull(Box);
        Assert.IsNotNull(PortraitLeft);
        Assert.IsNotNull(PortraitRight);
        Assert.IsNotNull(Background);
    }
}
