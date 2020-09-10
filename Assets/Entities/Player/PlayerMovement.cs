using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    enum State
    {
        Grounded,
        Jumping,
        Falling,
        Dashing,
    }

    State CurrentState = State.Grounded;
    bool IsGrounded => CurrentState == State.Grounded;
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
            CurrentState = State.Grounded;
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
        if (!IsDashing)
            LookDirection = LookDirection.FromFactor(MoveInput);

        var responsiveness = MoveResponsiveness;
        if (!IsGrounded)
            responsiveness = MoveResponsivenessInAir;

        var newMoveVelocity = MoveSpeed * MoveInput;
        MoveVelocity -= responsiveness * (MoveVelocity - newMoveVelocity);
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
            Jump();
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
        if (DashesLeft <= 0 || DashCooldownLeft > 0.0f)
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

        RigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(RigidBody);
    }

    void FixedUpdate()
    {
        UpdateCurrentState();

        UpdateMove();
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
