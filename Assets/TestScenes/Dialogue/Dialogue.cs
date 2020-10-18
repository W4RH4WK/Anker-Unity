using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour, ISubmitHandler
{
    public IEnumerator Say(string text)
    {
        Message.text = text;

        EventSystem.current.SetSelectedGameObject(gameObject);

        Continue = false;
        while (!Continue)
            yield return null;
    }

    public IEnumerator Show() => CoroutineUtils.Par(ShowMessageBox(), ShowPortrait());
    public IEnumerator Hide() => CoroutineUtils.Par(HideMessageBox(), HidePortrait());

    //////////////////////////////////////////////////////////////////////////

    public void OnSubmit(BaseEventData eventData) => Continue = true;
    bool Continue;

    //////////////////////////////////////////////////////////////////////////

    [Space]

    [SerializeField]
    RectTransform MessageBox;

    [SerializeField]
    float MessageBoxShowHideSpeed;

    Vector2 MessageBoxShowPosition;
    Vector2 MessageBoxHidePosition;

    public IEnumerator ShowMessageBox() => MessageBox.MoveTowardsAsync(MessageBoxShowPosition, MessageBoxShowHideSpeed);
    public IEnumerator HideMessageBox() => MessageBox.MoveTowardsAsync(MessageBoxHidePosition, MessageBoxShowHideSpeed);

    //////////////////////////////////////////////////////////////////////////

    [Space]

    [SerializeField]
    Text Message;

    [Space]

    [SerializeField]
    Image Portrait;

    [SerializeField]
    float PortraitShowHideSpeed;

    Vector2 PortraitShowPosition;
    Vector2 PortraitHidePosition;

    public IEnumerator ShowPortrait() => Portrait.rectTransform.MoveTowardsAsync(PortraitShowPosition,
                                                                                 PortraitShowHideSpeed);
    public IEnumerator HidePortrait() => Portrait.rectTransform.MoveTowardsAsync(PortraitHidePosition,
                                                                                 PortraitShowHideSpeed);

    //////////////////////////////////////////////////////////////////////////

    void Awake()
    {
        Assert.IsNotNull(MessageBox);
        MessageBoxShowPosition = MessageBox.anchoredPosition;
        MessageBoxHidePosition =
            MessageBoxShowPosition + Vector2.down * (MessageBox.rect.height + 2 * MessageBox.anchoredPosition.y);
        MessageBox.anchoredPosition = MessageBoxHidePosition;

        Assert.IsNotNull(Message);

        Assert.IsNotNull(Portrait);
        PortraitShowPosition = Portrait.rectTransform.anchoredPosition;
        PortraitHidePosition = PortraitShowPosition + Vector2.right * (Portrait.rectTransform.rect.width +
                                                                       -2 * Portrait.rectTransform.anchoredPosition.x);
        Portrait.rectTransform.anchoredPosition = PortraitHidePosition;
    }
}
