using UnityEngine;
using UnityEngine.Assertions;

public class Front : MonoBehaviour
{
    public float FadeSpeed;

    int InsideCount;

    SpriteRenderer Renderer;

    void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(Renderer);
    }

    void Update()
    {
        var targetOpacity = InsideCount > 0 ? 0.0f : 1.0f;

        var color = Renderer.color;
        color.a = Mathf.MoveTowards(color.a, targetOpacity, FadeSpeed * Time.deltaTime);
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
