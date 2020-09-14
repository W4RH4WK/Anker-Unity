using UnityEngine;

static class Vector3Extensions
{
    public static Vector2 AsVector2(this Vector3 v) => new Vector2(v.x, v.y);
}
