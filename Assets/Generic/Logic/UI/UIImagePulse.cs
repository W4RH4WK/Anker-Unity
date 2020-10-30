using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIImagePulse : MonoBehaviour
{
    public float Speed;

    public Color OtherColor;

    Image Image;

    void Awake()
    {
        Image = GetComponent<Image>();
        Assert.IsNotNull(Image);
    }

    void Update()
    {
        var percent = MathUtils.Sin01(Speed * Time.time);
        Image.color = Color.Lerp(Color.white, OtherColor, percent);
    }
}
