using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBackground : MonoBehaviour
{
    public IEnumerator On() => Animator.On(Duration);
    public IEnumerator Off() => Animator.Off(Duration);
    OnOffAnimator Animator;

    [SerializeField]
    float Duration;

    [SerializeField]
    float BlurStrength;

    [SerializeField]
    Color BlurColor;

    Image Image;

    void Awake()
    {
        Animator = new OnOffAnimator();

        Image = GetComponent<Image>();
        Assert.IsNotNull(Image);
        Image.material = new Material(Image.material);
    }

    void Update()
    {
        Image.color = Color.Lerp(Color.white, BlurColor, Animator.Percent);
        Image.material.SetFloat("_Strength", Mathf.Lerp(0.0f, BlurStrength, Animator.Percent));
    }
}
