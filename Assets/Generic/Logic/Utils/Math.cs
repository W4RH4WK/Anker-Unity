using UnityEngine;

static class Vector2Extensions
{
    public static Vector3 AsVector3(this Vector2 v) => new Vector3(v.x, v.y);

    public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        var tx = v.x;
        var ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector2 SnapAngle(this Vector2 v, int segments)
    {
        var snap = 360.0f / segments;

        var angle = Vector2.Angle(Vector2.right, v);
        var snappedAngle = Mathf.Round(angle / snap) * snap;
        return v.Rotate(angle - snappedAngle);
    }

    public static Vector2 SnapAngle4(this Vector2 v) => v.SnapAngle(4);
    public static Vector2 SnapAngle8(this Vector2 v) => v.SnapAngle(8);
}

static class Vector3Extensions
{
    public static Vector2 AsVector2(this Vector3 v) => new Vector2(v.x, v.y);

    public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}

static class MathUtils
{
    /// <summary>
    /// Remaps sinus such that Sin01(0) = 0 and Sin(1) = 1.
    /// </summary>
    public static float Sin01(float f) => 0.5f * Mathf.Sin((f - 0.5f) * Mathf.PI) + 0.5f;
}
