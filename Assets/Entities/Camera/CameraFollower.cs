using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollower : MonoBehaviour
{
    public GameObject Target;

    public float Distance;

    Camera Camera;

    void Awake()
    {
        Camera = GetComponent<Camera>();
        Assert.IsNotNull(Camera);
    }

    void Update()
    {
        if (Target)
        {
            var pos = Target.transform.position;
            Camera.transform.position = new Vector3(pos.x, pos.y, -Distance);
        }
    }
}
