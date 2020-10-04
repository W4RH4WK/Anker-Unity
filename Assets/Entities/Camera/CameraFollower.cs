using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject Target;
    public Vector2 Offset;

    public float SmoothTime;

    Vector3 Velocity;

    Vector3 TargetPosition()
    {
        if (!Target)
            return transform.position;

        var pos = Target.transform.position + Offset.AsVector3();
        pos.z = transform.position.z;
        return pos;
    }

    void Start()
    {
        transform.position = TargetPosition();
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition(), ref Velocity, SmoothTime);
    }
}
