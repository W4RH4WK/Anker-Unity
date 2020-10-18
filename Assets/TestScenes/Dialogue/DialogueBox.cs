using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public void SetName(string name) => Name.text = name;

    [SerializeField]
    Text Name;

    public void SetMessage(string message) => Message.text = message;

    [SerializeField]
    Text Message;

    public void Show() => Hider.Show();
    public void Hide() => Hider.Hide();
    public IEnumerator Show(float speed) => Hider.Show(speed);
    public IEnumerator Hide(float speed) => Hider.Hide(speed);
    OffscreenHider Hider;

    void Awake()
    {
        Assert.IsNotNull(Name);
        Assert.IsNotNull(Message);

        Hider = new OffscreenHider(GetComponent<RectTransform>());
    }
}
