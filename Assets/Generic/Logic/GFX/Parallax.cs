using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Vector2 Factor = Vector2.one;

    void Update()
    {
        var pos = (Vector2.one - Factor) * Camera.main.transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
    }
}
