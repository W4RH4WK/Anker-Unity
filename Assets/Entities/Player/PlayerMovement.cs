using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    public BoxCollider2D NormalCollider;
    public BoxCollider2D CrouchCollider;
    BoxCollider2D ActiveCollider => IsCrouching ? CrouchCollider : NormalCollider;

    public GameObject Body;

    Rect GetFeet()
    {
        var feet = new Rect();
        feet.position = ActiveCollider.bounds.center.AsVector2() + Vector2.down * ActiveCollider.size.y / 2.0f;
        feet.size = new Vector2(ActiveCollider.size.x, 0.01f);
        return feet;
    }

    Rect GetHead()
    {
        var head = new Rect();
        head.position = ActiveCollider.bounds.center.AsVector2() + Vector2.up * ActiveCollider.size.y / 2.0f;
        head.size = new Vector2(ActiveCollider.size.x, 0.01f);
        return head;
    }

    public Orientation LookDirection { get; private set; } = Orientation.Right;

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float MoveSpeed;
    public float MoveResponsiveness;
    public float MoveResponsivenessInAir;
    float MoveVelocity;

    Vector2 MoveInput;
    void OnMove(InputValue value) => MoveInput = value.Get<Vector2>();

    void UpdateMove()
    {
        if (!IsDashing && !IsCrouchSliding)
            LookDirection = LookDirection.FromFactor(MoveInput.x);

        if (IsCrouching)
        {
            MoveVelocity = 0.0f;
            return;
        }

        var responsiveness = MoveResponsiveness;
        if (!IsGrounded)
            responsiveness = MoveResponsivenessInAir;

        var newMoveVelocity = MoveSpeed * MoveInput.x;
        MoveVelocity -= responsiveness * (MoveVelocity - newMoveVelocity);
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float CrouchSlideSpeed;
    public float CrouchSlideTime;
    float CrouchSlideTimeLeft;
    public float CrouchSlideCooldown;
    float CrouchSlideCooldownLeft;

    public bool IsCrouching => IsGrounded && CrouchInput;
    bool IsCrouchSliding => CrouchSlideTimeLeft > 0.0f;

    bool CrouchInput => MoveInput.y < -0.5f;

    void CrouchSlide()
    {
        if (CrouchSlideTimeLeft > 0.0f || CrouchSlideCooldownLeft > 0.0f || !IsCrouching)
            return;

        CrouchSlideTimeLeft = CrouchSlideTime;
        CrouchSlideCooldownLeft = CrouchSlideCooldown;
    }

    void UpdateCrouch()
    {
        CrouchSlideCooldownLeft -= Time.fixedDeltaTime;
        CrouchSlideTimeLeft -= Time.fixedDeltaTime;

        NormalCollider.enabled = !IsCrouching;
        CrouchCollider.enabled = IsCrouching;

        // TODO: Current body is only a placeholder until we have dedicated
        // sprites.
        if (IsCrouching)
        {
            Body.transform.localPosition = new Vector3(0.0f, 0.25f);
            Body.transform.localScale = new Vector3(1.2f, 0.5f, 1.0f);
        }
        else
        {
            Body.transform.localPosition = new Vector3(0.0f, 0.75f);
            Body.transform.localScale = new Vector3(1.0f, 1.5f, 1.0f);
        }

        // Stop crouch slide when sliding over edge.
        if (IsFalling)
            CrouchSlideTimeLeft = 0.0f;

        if (CrouchSlideTimeLeft > 0.0f)
            MoveVelocity = LookDirection.ToFactor() * CrouchSlideSpeed;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float Gravity;
    public float MaxFallSpeed;
    float VerticalVelocity;

    bool IsGrounded;
    ContactFilter2D TerrainContactFilter;

    void UpdateIsGrounded()
    {
        if (VerticalVelocity > 0.0f)
        {
            IsGrounded = false;
        }
        else if (PlatformTracker.HasContact)
        {
            IsGrounded = true;
        }
        else
        {
            var feet = GetFeet();
            IsGrounded = Physics2DUtils.BoxCast(feet, Vector2.down, feet.size.y, TerrainContactFilter);
        }
    }

    void UpdateFalling()
    {
        if (IsJumping || IsDashing || IsGroundSlamming)
            return;

        if (IsGrounded)
            VerticalVelocity = 0.0f;
        else
            VerticalVelocity -= Gravity * Time.fixedDeltaTime;

        VerticalVelocity = Mathf.Max(VerticalVelocity, -MaxFallSpeed);
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public int Jumps;
    int JumpsLeft;
    public float JumpVelocity;
    public float DoubleJumpVelocity;
    public float JumpReleaseGravity;

    bool IsJumping => VerticalVelocity > 0.0f;
    bool IsFalling => VerticalVelocity < 0.0f;

    public float CoyoteTime;
    float CoyoteTimeLeft;

    bool JumpInput;
    void OnJump(InputValue value)
    {
        JumpInput = value.isPressed;
        if (value.isPressed)
        {
            if (IsCrouching)
            {
                if (PlatformTracker.HasContact)
                    PlatformTracker.DropThrough();
                else
                    CrouchSlide();
            }
            else
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        if (JumpsLeft <= 0)
            return;

        if (JumpsLeft == Jumps)
            VerticalVelocity = JumpVelocity;
        else
            VerticalVelocity = DoubleJumpVelocity;

        JumpsLeft--;
    }

    void UpdateJump()
    {
        // update coyote time
        if (IsGrounded)
            CoyoteTimeLeft = CoyoteTime;
        else
            CoyoteTimeLeft -= Time.fixedDeltaTime;

        // update jumps
        if (IsGrounded)
            JumpsLeft = Jumps;

        // We loose one jump when falling (and coyote-time has passed).
        if (IsFalling && CoyoteTimeLeft <= 0.0f && JumpsLeft == Jumps)
            JumpsLeft--;

        if (IsJumping)
        {
            if (JumpInput)
                VerticalVelocity -= Gravity * Time.fixedDeltaTime;
            else
                VerticalVelocity -= JumpReleaseGravity * Time.fixedDeltaTime;

            // Prevent sticking to ceiling.
            {
                var head = GetHead();
                if (Physics2DUtils.BoxCast(head, Vector2.up, head.size.y, TerrainContactFilter))
                    VerticalVelocity = 0;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public int Dashes;
    int DashesLeft;
    public float DashSpeed;
    public float DashTime;
    float DashTimeLeft;
    public float DashCooldown;
    float DashCooldownLeft;

    Orientation DashDirection;

    bool IsDashing => DashTimeLeft > 0.0f;

    void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            if (CrouchInput)
                GroundSlam();
            else
                Dash();
        }
    }

    void Dash()
    {
        if (DashesLeft <= 0 || DashCooldownLeft > 0.0f || IsCrouching)
            return;

        DashDirection = LookDirection;

        // Backdash on ground without move input.
        if (IsGrounded && Mathf.Abs(MoveInput.x) <= 0.1f)
            DashDirection = LookDirection.Inverse();

        DashesLeft--;
        DashTimeLeft = DashTime;
        DashCooldownLeft = DashCooldown;
    }

    void UpdateDash()
    {
        DashCooldownLeft -= Time.fixedDeltaTime;
        DashTimeLeft -= Time.fixedDeltaTime;

        if (IsGrounded)
            DashesLeft = Dashes;

        if (IsDashing)
            MoveVelocity = DashDirection.ToFactor() * DashSpeed;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float GroundSlamSpeed;
    bool IsGroundSlamming;

    void GroundSlam()
    {
        IsGroundSlamming = true;
    }

    void UpdateGroundSlam()
    {
        if (!IsGroundSlamming)
            return;

        if (IsGrounded)
            IsGroundSlamming = false;
        else
            VerticalVelocity = -GroundSlamSpeed;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float AnchorReelSpeed;
    public float AnchorReelBreakoffDistance;
    AnchorPoint ActiveAnchorPoint;

    AnchorPointSelector AnchorPointSelector;

    void OnAnchor() => ActiveAnchorPoint = AnchorPointSelector.GetAnchorPoint(MoveInput, LookDirection);

    void UpdateAnchor()
    {
        if (!ActiveAnchorPoint)
            return;

        var toPoint = ActiveAnchorPoint.transform.position.AsVector2() - RigidBody.position;
        if (toPoint.magnitude < AnchorReelBreakoffDistance)
        {
            ActiveAnchorPoint = null;
        }
        else
        {
            var velocity = toPoint.normalized * AnchorReelSpeed;
            MoveVelocity = velocity.x;
            VerticalVelocity = velocity.y;

            Debug.DrawLine(RigidBody.position, ActiveAnchorPoint.transform.position);
        }
    }

    //////////////////////////////////////////////////////////////////////////

    Rigidbody2D RigidBody;

    PlatformTracker PlatformTracker;

    void Awake()
    {
        Assert.IsNotNull(NormalCollider);
        Assert.IsNotNull(CrouchCollider);

        TerrainContactFilter = new ContactFilter2D();
        TerrainContactFilter.SetLayerMask(LayerMask.GetMask("Terrain"));

        RigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(RigidBody);

        AnchorPointSelector = GetComponentInChildren<AnchorPointSelector>();
        Assert.IsNotNull(AnchorPointSelector);

        PlatformTracker = GetComponentInChildren<PlatformTracker>();
        Assert.IsNotNull(PlatformTracker);
    }

    void FixedUpdate()
    {
        UpdateIsGrounded();

        UpdateMove();
        UpdateCrouch();
        UpdateGroundSlam();
        UpdateFalling();
        UpdateJump();
        UpdateDash();
        UpdateAnchor();

        RigidBody.velocity = new Vector2(MoveVelocity, VerticalVelocity);
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

        GUILayout.Window(0, new Rect(), windowFunc, "Player Tweaks", GUILayout.Width(300));
    }
#endif
}
