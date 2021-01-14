using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class Filament : MonoBehaviour, ISubmitHandler
{
    public void OnSubmit(BaseEventData eventData) => ContinueLastFrame = Time.frameCount;
    bool Continue => ContinueLastFrame == Time.frameCount;
    int ContinueLastFrame;

    public IEnumerator WaitForContinue()
    {
        // Wait for at least one frame.
        yield return null;

        while (!Continue)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            yield return null;
        }

        AudioSource.PlayOneShot(ClickSound, 0.6f);
    }

    public DialogueSystem DialogueSystem { get; private set; }
    public DocumentSystem DocumentSystem { get; private set; }

    [SerializeField] AudioClip ClickSound;

    AudioSource AudioSource;
    void Awake()
    {
        DialogueSystem = FindObjectOfType<DialogueSystem>();
        Assert.IsNotNull(DialogueSystem);

        DocumentSystem = FindObjectOfType<DocumentSystem>();
        Assert.IsNotNull(DocumentSystem);

        Assert.IsNotNull(ClickSound);

        AudioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(AudioSource);
    }
}
