using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Vector2 Factor;
    Vector2 Offset;

    void Awake()
    {
        Offset = transform.position;
    }

    void Update()
    {
        var pos = Factor * Camera.main.transform.position + Offset;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
