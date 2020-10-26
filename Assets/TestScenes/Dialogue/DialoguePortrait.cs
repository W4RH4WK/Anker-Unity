using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialoguePortrait : MonoBehaviour
{
    public IEnumerator Set(Sprite sprite)
    {
        if (Image.sprite != sprite)
            yield return HideAsync();

        Image.sprite = sprite;
        yield return ShowAsync();
    }

    public void Show() => Hider.Show();
    public void Hide() => Hider.Hide();
    public IEnumerator ShowAsync() => Hider.ShowAsync();
    public IEnumerator HideAsync() => Hider.HideAsync();
    CanvasGroupHider Hider;

    Image Image;

    void Awake()
    {
        Image = GetComponent<Image>();
        Assert.IsNotNull(Image);

        Hider = GetComponent<CanvasGroupHider>();
        Assert.IsNotNull(Hider);
    }
}
