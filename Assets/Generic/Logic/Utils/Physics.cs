using UnityEngine;

static class Physics2DUtils
{
    public static bool BoxCast(Rect rect, Vector2 direction, float distance, ContactFilter2D contactFilter)
    {
        return Physics2D.BoxCast(rect.position, rect.size, 0.0f, direction, contactFilter, new RaycastHit2D[1],
                                 distance) > 0;
    }
}
