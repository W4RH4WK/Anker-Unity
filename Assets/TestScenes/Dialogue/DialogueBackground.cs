using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DialogueBackground : MonoBehaviour
{
    public IEnumerator On() => Animator.On(System.AnimationDuration);
    public IEnumerator Off() => Animator.Off(System.AnimationDuration);
    OnOffAnimator Animator;

    [SerializeField]
    float BlurStrength;

    [SerializeField]
    Color BlurColor;

    DialogueSystem System;

    Image Image;

    void Awake()
    {
        System = GetComponentInParent<DialogueSystem>();
        Assert.IsNotNull(System);

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
