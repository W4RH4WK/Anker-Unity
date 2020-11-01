using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class DialogueSystem : MonoBehaviour, ISubmitHandler
{
    public IEnumerator Say(string text)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);

        yield return this.Par(Box.ShowAsync(), Box.SetMessage(text), Background.On());

        Continue = false;
        while (!Continue)
            yield return null;

        AudioSource.PlayOneShot(ClickSound, 0.6f);
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

    IEnumerator SetPortraitLeft(Sprite image) => PortraitLeft.Set(image);
    IEnumerator SetPortraitRight(Sprite image) => PortraitRight.Set(image);

    public IEnumerator Hide() => this.Par(Box.HideAsync(), PortraitLeft.HideAsync(), PortraitRight.HideAsync(),
                                          Background.Off());

    //////////////////////////////////////////////////////////////////////////

    public void OnSubmit(BaseEventData eventData) => Continue = true;
    bool Continue;

    //////////////////////////////////////////////////////////////////////////

    [Header("Settings")]

    public float AnimationDuration;

    public AudioClip ClickSound;

    [Header("Elements")]

    [SerializeField]
    DialogueBox Box;

    [SerializeField]
    DialoguePortrait PortraitLeft;

    [SerializeField]
    DialoguePortrait PortraitRight;

    [SerializeField]
    DialogueBackground Background;

    AudioSource AudioSource;

    void Awake()
    {
        Assert.IsNotNull(Box);
        Assert.IsNotNull(PortraitLeft);
        Assert.IsNotNull(PortraitRight);
        Assert.IsNotNull(Background);
        Assert.IsNotNull(ClickSound);

        AudioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(AudioSource);
    }
}
