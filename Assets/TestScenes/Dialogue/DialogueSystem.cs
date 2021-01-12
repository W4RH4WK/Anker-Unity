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
        yield return WaitForContinue();
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

    public IEnumerator ShowDocument(Document document)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);

        yield return DocumentBackground.ShowAsync(DocumentTextRevealDuration);
        yield return Document.ShowAsync();
        yield return Document.SetText(document.Title, document.Text);
        yield return Document.HideAsync();
        yield return DocumentBackground.HideAsync(DocumentTextRevealDuration);
    }

    public IEnumerator HidePortraitLeft() => PortraitLeft.HideAsync();
    public IEnumerator HidePortraitRight() => PortraitRight.HideAsync();

    IEnumerator SetPortraitLeft(Sprite image) => PortraitLeft.Set(image);
    IEnumerator SetPortraitRight(Sprite image) => PortraitRight.Set(image);

    public IEnumerator Hide() => this.Par(Box.HideAsync(), PortraitLeft.HideAsync(), PortraitRight.HideAsync(),
                                          Background.Off());

    //////////////////////////////////////////////////////////////////////////

    public void OnSubmit(BaseEventData eventData) => ContinueLastFrame = Time.frameCount;
    bool Continue => ContinueLastFrame == Time.frameCount;
    int ContinueLastFrame;

    public IEnumerator WaitForContinue()
    {
        // Wait for at least one frame.
        yield return null;

        while (!Continue)
            yield return null;

        AudioSource.PlayOneShot(ClickSound, 0.6f);
    }

    //////////////////////////////////////////////////////////////////////////

    [Header("Settings")]

    public float AnimationDuration;
    public float DocumentTextRevealDuration;
    public float DocumentTextFadedOutAlpha;

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

    [SerializeField]
    DocumentBox Document;

    [SerializeField]
    CanvasGroupHider DocumentBackground;

    AudioSource AudioSource;

    void Awake()
    {
        Assert.IsNotNull(ClickSound);

        Assert.IsNotNull(Box);
        Assert.IsNotNull(PortraitLeft);
        Assert.IsNotNull(PortraitRight);
        Assert.IsNotNull(Background);
        Assert.IsNotNull(Document);
        Assert.IsNotNull(DocumentBackground);

        AudioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(AudioSource);
    }
}
