using UnityEngine;

/// <summary>
/// Orientation states the direction a sprite is facing.
/// </summary>
enum Orientation
{
    Left,
    Right,
}

static class OrientationExtensions
{
    public static Orientation Inverse(this Orientation orientation)
    {
        if (orientation == Orientation.Left)
            return Orientation.Right;
        else
            return Orientation.Left;
    }

    /// <summary>
    /// Left if <0, Right if >0, unchanged otherwise.
    /// </summary>
    public static Orientation FromFactor(this Orientation orientation, float value)
    {
        if (value > 0.0f)
            return Orientation.Right;
        else if (value < 0.0f)
            return Orientation.Left;
        else
            return orientation;
    }

    /// <summary>
    /// Returns -1 when facing left, otherwise +1.
    /// </summary>
    public static float ToFactor(this Orientation orientation)
    {
        if (orientation == Orientation.Left)
            return -1.0f;
        else
            return 1.0f;
    }

    /// <summary>
    /// Returns left or right facing Vector2.
    /// </summary>
    public static Vector2 ToVector2(this Orientation orientation)
    {
        if (orientation == Orientation.Left)
            return Vector2.left;
        else
            return Vector2.right;
    }
}
