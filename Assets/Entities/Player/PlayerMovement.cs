using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    enum State
    {
        Standing,
        Crouching,
        Jumping,
        Falling,
        Dashing,
    }

    State CurrentState = State.Standing;
    bool IsGrounded => CurrentState == State.Standing || CurrentState == State.Crouching;
    bool IsCrouching => CurrentState == State.Crouching;
    bool IsDashing => CurrentState == State.Dashing;

    Orientation LookDirection = Orientation.Right;

    public Collider2D Head;
    public Collider2D Feet;
    ContactFilter2D TerrainContactFilter;

    void UpdateCurrentState()
    {
        if (DashTimeLeft > 0.0f)
            CurrentState = State.Dashing;
        else if (VerticalVelocity > 0.0f)
            CurrentState = State.Jumping;
        else if (Physics2D.OverlapCollider(Feet, TerrainContactFilter, new Collider2D[1]) > 0)
            CurrentState = CrouchInput ? State.Crouching : State.Standing;
        else
            CurrentState = State.Falling;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float MoveSpeed;
    public float MoveResponsiveness;
    public float MoveResponsivenessInAir;
    float MoveVelocity;

    float MoveInput;
    void OnMove(InputValue value) => MoveInput = value.Get<float>();

    void UpdateMove()
    {
        if (!IsDashing && !IsCrouchSliding)
            LookDirection = LookDirection.FromFactor(MoveInput);

        if (IsCrouching)
        {
            MoveVelocity = 0.0f;
            return;
        }

        var responsiveness = MoveResponsiveness;
        if (!IsGrounded)
            responsiveness = MoveResponsivenessInAir;

        var newMoveVelocity = MoveSpeed * MoveInput;
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

    bool IsCrouchSliding => CrouchSlideTimeLeft > 0.0f;

    bool CrouchInput;
    void OnCrouch(InputValue value) => CrouchInput = value.isPressed;

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

        // Stop crouch slide when sliding over edge.
        if (CurrentState == State.Falling)
            CrouchSlideTimeLeft = 0.0f;

        if (IsGrounded && CrouchInput)
            transform.localScale = CrouchScale;
        else
            transform.localScale = BaseScale;

        if (CrouchSlideTimeLeft > 0.0f)
            MoveVelocity = LookDirection.ToFactor() * CrouchSlideSpeed;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float Gravity;
    public float MaxFallSpeed;
    float VerticalVelocity;

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public int Jumps;
    int JumpsLeft;
    public float JumpStrength;
    public float JumpBoostStrength;
    public float JumpBoostTime;
    float JumpBoostTimeLeft;

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
        if (CurrentState == State.Falling && CoyoteTimeLeft <= 0.0f && JumpsLeft == Jumps)
            JumpsLeft--;

        if (CurrentState == State.Jumping)
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

    void OnDash(InputValue value)
    {
        if (value.isPressed)
            Dash();
    }

    void Dash()
    {
        if (DashesLeft <= 0 || DashCooldownLeft > 0.0f || IsCrouching)
            return;

        DashDirection = LookDirection;

        // Backdash on ground without move input.
        if (IsGrounded && Mathf.Abs(MoveInput) <= 0.1f)
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

    Rigidbody2D RigidBody;

    void Awake()
    {
        Assert.IsNotNull(Head);
        Assert.IsNotNull(Feet);

        TerrainContactFilter = new ContactFilter2D();
        TerrainContactFilter.SetLayerMask(LayerMask.GetMask("Terrain"));

        BaseScale = transform.localScale;

        RigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(RigidBody);
    }

    void FixedUpdate()
    {
        UpdateCurrentState();

        UpdateMove();
        UpdateCrouch();
        UpdateJump();
        UpdateDash();

        // Add slight downward velocity to prevent peter-panning.
        if (IsGrounded)
            VerticalVelocity = -0.1f;

        if (CurrentState == State.Falling)
            VerticalVelocity -= Gravity * Time.fixedDeltaTime;

        VerticalVelocity = Mathf.Max(VerticalVelocity, -MaxFallSpeed);

        RigidBody.velocity = new Vector2(MoveVelocity, VerticalVelocity);
    }
}
