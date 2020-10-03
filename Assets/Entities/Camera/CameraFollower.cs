using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject Target;
    public Vector2 Offset;

    public float SmoothTime;

    [Space]

    public float PlayerLookOffset;

    Vector3 Velocity;

    Vector3 TargetPosition()
    {
        if (!Target)
            return transform.position;

        var result = Target.transform.position + new Vector3(Offset.x, Offset.y, transform.position.z);

        // Special handling for player. Offset the camera a little bit according
        // to its orientation.
        {
            var player = Target.GetComponent<PlayerMovement>();
            if (player)
                result += PlayerLookOffset * player.LookDirection.ToVector3();
        }

        return result;
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
