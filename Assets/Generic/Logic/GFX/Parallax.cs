using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Vector2 Factor;

    void Update()
    {
        var pos = Factor * Camera.main.transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
    }
}
