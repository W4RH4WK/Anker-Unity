using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialoguePortrait : MonoBehaviour
{
    public void Set(Sprite sprite) => Image.sprite = sprite;
    Image Image;

    public void Show() => Hider.Show();
    public void Hide() => Hider.Hide();
    public IEnumerator Show(float speed) => Hider.Show(speed);
    public IEnumerator Hide(float speed) => Hider.Hide(speed);
    OffscreenHider Hider;

    void Awake()
    {
        Image = GetComponent<Image>();
        Assert.IsNotNull(Image);

        Hider = new OffscreenHider(GetComponent<RectTransform>());
    }
}
