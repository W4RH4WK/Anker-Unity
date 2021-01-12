using UnityEngine;
using UnityEngine.Assertions;

public class UISpinner : MonoBehaviour
{
    public float Speed;

    RectTransform Transform;

    void Awake()
    {
        Transform = GetComponent<RectTransform>();
        Assert.IsNotNull(Transform);
    }

    void Update()
    {
        Transform.Rotate(Vector3.forward, -Speed * Time.deltaTime);
    }
}
