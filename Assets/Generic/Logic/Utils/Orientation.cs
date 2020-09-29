/// <summary>
/// Orientation states the direction a sprite is facing.
/// </summary>
public enum Orientation
{
    Left,
    Right,
}

public static class OrientationExtensions
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
}
