using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Collider2D Head;
    public Collider2D Feet;

    Orientation LookDirection = Orientation.Right;

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

    public Vector3 CrouchScale;
    Vector3 BaseScale;
    public float CrouchSlideSpeed;
    public float CrouchSlideTime;
    float CrouchSlideTimeLeft;
    public float CrouchSlideCooldown;
    float CrouchSlideCooldownLeft;

    bool IsCrouching => IsGrounded && CrouchInput;
    bool IsCrouchSliding => CrouchSlideTimeLeft > 0.0f;

    bool CrouchInput => MoveInput.y < -0.5f;
    //void OnCrouch(InputValue value) => CrouchInput = value.isPressed;

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

        if (IsCrouching)
            transform.localScale = CrouchScale;
        else
            transform.localScale = BaseScale;

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
        IsGrounded = Physics2D.OverlapCollider(Feet, TerrainContactFilter, new Collider2D[1]) > 0;
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
    public float JumpStrength;
    public float JumpBoostStrength;
    public float JumpBoostTime;
    float JumpBoostTimeLeft;

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
                CrouchSlide();
            else
                Jump();
        }
    }

    void Jump()
    {
        if (JumpsLeft <= 0)
            return;

        JumpsLeft--;

        VerticalVelocity = JumpStrength;
        JumpBoostTimeLeft = JumpBoostTime;
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

        // update jump boost time
        if (IsGrounded)
            JumpBoostTimeLeft = JumpBoostTime;
        else if (!JumpInput)
            JumpBoostTimeLeft = 0.0f;
        else
            JumpBoostTimeLeft -= Time.fixedDeltaTime;

        // We loose one jump when falling (and coyote-time has passed).
        if (IsFalling && CoyoteTimeLeft <= 0.0f && JumpsLeft == Jumps)
            JumpsLeft--;

        if (IsJumping)
        {
            if (JumpBoostTimeLeft > 0.0f)
                VerticalVelocity += JumpBoostStrength * Time.fixedDeltaTime;
            else
                VerticalVelocity -= Gravity * Time.fixedDeltaTime;

            // Prevent sticking to ceiling.
            if (Physics2D.OverlapCollider(Head, TerrainContactFilter, new Collider2D[1]) > 0)
                VerticalVelocity = 0;
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
    bool IsReelingIn => ActiveAnchorPoint;

    AnchorRadius AnchorRadiusScript;

    Vector2 DebugAnchorInput;

    void OnAnchor(InputValue value)
    {
        var input = value.Get<Vector2>();

        if (input.magnitude > 0.0f)
            DebugAnchorInput = input.normalized;

        if (input.magnitude == 1.0f)
        {
            var pointsInRange = AnchorRadiusScript.AnchorPointsInRange;
            if (pointsInRange.Count > 0)
                ActiveAnchorPoint = pointsInRange[0];
        }
    }

    void UpdateAnchor()
    {
        Debug.DrawLine(RigidBody.position, 3.0f * DebugAnchorInput + RigidBody.position);

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

    void Awake()
    {
        Assert.IsNotNull(Head);
        Assert.IsNotNull(Feet);

        TerrainContactFilter = new ContactFilter2D();
        TerrainContactFilter.SetLayerMask(LayerMask.GetMask("Terrain"));

        BaseScale = transform.localScale;

        AnchorRadiusScript = GetComponentInChildren<AnchorRadius>();
        Assert.IsNotNull(AnchorRadiusScript);

        RigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(RigidBody);
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
}
