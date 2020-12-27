using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraPlayerFollower : MonoBehaviour
{
    public Vector2 Offset;

    public float LeftRightOffset;
    public float CrouchOffset;

    public float SmoothHorizontal;
    public float SmoothVertical;
    public float SmoothFalling;

    PlayerMovement Player;

    Rigidbody2D Body;

    Vector3 TargetPosition()
    {
        var targetPosition = Player.transform.position + Offset.AsVector3();
        targetPosition.z = transform.position.z;

        targetPosition.x += Player.LookDirection.ToFactor() * LeftRightOffset;

        if (Player.IsCrouching)
            targetPosition.y -= CrouchOffset;

        return targetPosition;
    }

    void OnEnable()
    {
        Player = FindObjectOfType<PlayerMovement>();
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
        if (!Player)
        {
            enabled = false;
            return;
        }

        var targetPosition = TargetPosition();
        var bodyPosition = Body.position;

        var smoothVertical = SmoothVertical;
        if (Player.IsFalling)
            smoothVertical = SmoothFalling;

        bodyPosition.x = Mathf.Lerp(bodyPosition.x, targetPosition.x, Time.fixedDeltaTime / SmoothHorizontal);
        bodyPosition.y = Mathf.Lerp(bodyPosition.y, targetPosition.y, Time.fixedDeltaTime / smoothVertical);
        Body.position = bodyPosition;
    }

#if !UNITY_EDITOR
    void OnGUI()
    {
        GUI.WindowFunction windowFunc = (int windowId) =>
        {
            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public |
                                                      BindingFlags.Instance))
            {
                GUILayout.BeginHorizontal();
                if (field.FieldType == typeof(float))
                {
                    GUILayout.Label(field.Name);
                    field.SetValue(this, float.Parse(GUILayout.TextField($"{(float)field.GetValue(this)}")));
                }
                GUILayout.EndHorizontal();
            }
        };

        GUILayout.Window(1, new Rect(300, 0, 0, 0), windowFunc, "Camera Tweaks", GUILayout.Width(300));
    }
#endif
}
