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

#if !UNITY_EDITOR
    void OnGUI()
    {
        GUI.WindowFunction windowFunc = (int windowId) =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("SmoothTime");
            SmoothTime = float.Parse(GUILayout.TextField($"{SmoothTime}"));
            GUILayout.EndHorizontal();
        };

        GUILayout.Window(1, new Rect(300, 0, 0, 0), windowFunc, "Camera Tweaks", GUILayout.Width(300));
    }
#endif
}
