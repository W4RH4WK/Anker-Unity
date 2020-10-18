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

        yield return Show();

        Continue = false;
        while (!Continue)
            yield return null;
    }

    public IEnumerator Say(string name, string text)
    {
        Box.SetName(name);
        return Say(text);
    }

    public IEnumerator Show() => CoroutineUtils.Par(Box.Show(ShowHideSpeed), Portrait.Show(ShowHideSpeed));
    public IEnumerator Hide() => CoroutineUtils.Par(Box.Hide(ShowHideSpeed), Portrait.Hide(ShowHideSpeed));

    //////////////////////////////////////////////////////////////////////////

    public void OnSubmit(BaseEventData eventData) => Continue = true;
    bool Continue;

    //////////////////////////////////////////////////////////////////////////

    [SerializeField]
    DialogueBox Box;

    [SerializeField]
    DialoguePortrait Portrait;

    [SerializeField]
    float ShowHideSpeed;

    void Awake()
    {
        Assert.IsNotNull(Box);
        Box.Hide();

        Assert.IsNotNull(Portrait);
        Portrait.Hide();
    }
}
