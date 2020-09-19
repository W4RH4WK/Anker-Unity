using UnityEngine;
using UnityEngine.Assertions;

public class Front : MonoBehaviour
{
    public float FadeSpeed;

    float TargetOpacity = 1.0f;

    int InsideCount;

    SpriteRenderer Renderer;

    void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(Renderer);
    }

    void Update()
    {
        if (InsideCount > 0)
            TargetOpacity = 0.0f;
        else
            TargetOpacity = 1.0f;

        var color = Renderer.color;
        color.a = Mathf.MoveTowards(color.a, TargetOpacity, FadeSpeed * Time.deltaTime);
        Renderer.color = color;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponentInParent<PlayerMovement>())
            InsideCount++;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponentInParent<PlayerMovement>())
            InsideCount--;
    }
}
