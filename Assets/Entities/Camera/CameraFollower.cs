using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollower : MonoBehaviour
{
    public GameObject Target;
    public Vector2 Offset;

    public float Smoothing;

    public bool Breathing;
    public float BreathingAmplitude;
    public float BreathingFrequency;

    Rigidbody2D Body;

    Vector3 TargetPosition()
    {
        if (!Target)
            return transform.position;

        var pos = Target.transform.position + Offset.AsVector3();
        pos.z = transform.position.z;
        return pos;
    }

    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(Body);
    }

    void Start()
    {
        Body.position = TargetPosition();
    }

    void FixedUpdate()
    {
        var breathingOffset = Vector3.zero;
        if (Breathing)
        {
            breathingOffset = BreathingAmplitude *
                              Mathf.Sin(2.0f * Mathf.PI * BreathingFrequency * Time.timeSinceLevelLoad) * Vector3.up;
        }

        Body.position =
            Vector3.Lerp(Body.position, TargetPosition() + breathingOffset, Time.fixedDeltaTime / Smoothing);
    }

#if !UNITY_EDITOR
    void OnGUI()
    {
        GUI.WindowFunction windowFunc = (int windowId) =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Smoothing");
            Smoothing = float.Parse(GUILayout.TextField($"{Smoothing}"));
            GUILayout.EndHorizontal();
        };

        GUILayout.Window(1, new Rect(300, 0, 0, 0), windowFunc, "Camera Tweaks", GUILayout.Width(300));
    }
#endif
}
