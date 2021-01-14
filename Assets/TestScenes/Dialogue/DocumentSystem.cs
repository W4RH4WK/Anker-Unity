using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class DocumentSystem : MonoBehaviour
{
    public IEnumerator Show(Document document)
    {
        yield return Background.ShowAsync(TextRevealDuration);
        yield return Box.ShowAsync();
        yield return Box.Set(document.Title, document.Body);
        yield return Box.HideAsync();
        yield return Background.HideAsync(TextRevealDuration);
    }

    public float AnimationDuration;
    public float TextRevealDuration;
    public float TextFadedOutAlpha;

    [SerializeField]
    DocumentBox Box;

    [SerializeField]
    CanvasGroupHider Background;

    void Awake()
    {
        Assert.IsNotNull(Box);
        Assert.IsNotNull(Background);
    }
}
