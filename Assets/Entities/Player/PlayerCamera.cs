using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 Offset;

    void Update()
    {
        Camera.main.transform.position = transform.position + Offset;
    }
}
