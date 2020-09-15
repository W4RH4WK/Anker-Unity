using UnityEngine;

static class Vector2Extensions
{
    public static Vector3 AsVector3(this Vector2 v) => new Vector3(v.x, v.y);
}

static class Vector3Extensions
{
    public static Vector2 AsVector2(this Vector3 v) => new Vector2(v.x, v.y);
}
