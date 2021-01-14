using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DialogueSystem : MonoBehaviour
{
    public IEnumerator Say(string text)
    {
        yield return this.Par(Box.ShowAsync(), Box.SetMessage(text), Background.On());
        yield return FindObjectOfType<Filament>().WaitForContinue(); // FIXME
    }

    public IEnumerator Say(string name, string text) => this.Par(Box.SetName(name), Say(text));

    public IEnumerator Say(DialogueCharacter character,
                           string text) => this.Par(PortraitLeft.RaiseAsync(), PortraitRight.LowerAsync(),
                                                    SetPortraitLeft(character.Image), Say(character.Name, text));

    public IEnumerator SayRight(DialogueCharacter character,
                                string text) => this.Par(PortraitLeft.LowerAsync(), PortraitRight.RaiseAsync(),
                                                         SetPortraitRight(character.Image), Say(character.Name, text));

    public IEnumerator Tell(string text) => this.Par(Box.HideName(), Say(text), PortraitLeft.LowerAsync(),
                                                     PortraitRight.LowerAsync());

    public IEnumerator HidePortraitLeft() => PortraitLeft.HideAsync();
    public IEnumerator HidePortraitRight() => PortraitRight.HideAsync();

    IEnumerator SetPortraitLeft(Sprite image) => PortraitLeft.Set(image);
    IEnumerator SetPortraitRight(Sprite image) => PortraitRight.Set(image);

    public IEnumerator Hide() => this.Par(Box.HideAsync(), PortraitLeft.HideAsync(), PortraitRight.HideAsync(),
                                          Background.Off());

    //////////////////////////////////////////////////////////////////////////

    [Header("Settings")]

    public float AnimationDuration;

    [Header("Elements")]

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
