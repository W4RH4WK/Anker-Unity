using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCameraTarget : MonoBehaviour
{
    public float CrouchOffset;

    Vector2 BaseOffset;

    PlayerMovement Player;

    void Awake()
    {
        BaseOffset = transform.localPosition;

        Player = GetComponentInParent<PlayerMovement>();
        Assert.IsNotNull(Player);
    }

    void Update()
    {
        var pos = BaseOffset;
        pos.x *= Player.LookDirection.ToFactor();

        if (Player.IsCrouching)
            pos.y -= CrouchOffset;

        transform.localPosition = pos;
    }
}
