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

    enum Orientation
    {
        Right,
        Left,
    }

    static Orientation InvertOrientation(Orientation orientation)
    {
        switch (orientation)
        {
        case Orientation.Left:
            return Orientation.Right;
        case Orientation.Right:
            return Orientation.Left;
        }
        return Orientation.Left;
    }

    Orientation LookDirection = Orientation.Right;

    public Collider2D Head;
    public Collider2D Feet;
    ContactFilter2D TerrainContactFilter;

    void UpdateCurrentState()
    {
        if (DashTimeLeft > 0.0f)
            CurrentState = State.Dashing;
        else if (Physics2D.OverlapCollider(Feet, TerrainContactFilter, new Collider2D[1]) > 0)
            CurrentState = State.Grounded;
        else if (VerticalVelocity > 0.0f)
            CurrentState = State.Jumping;
        else
            CurrentState = State.Falling;

        // Prevent sticking to ceiling.
        if (CurrentState == State.Jumping)
        {
            if (Physics2D.OverlapCollider(Head, TerrainContactFilter, new Collider2D[1]) > 0)
            {
                CurrentState = State.Falling;
                VerticalVelocity = 0;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float MoveSpeed;
    public float MoveResponsiveness;
    public float MoveResponsivenessInAir;
    float MoveVelocity;

    float MoveInput;
    void OnMove(InputValue value) => MoveInput = value.Get<float>();

    void UpdateMoveVelocity()
    {
        if (!IsDashing)
        {
            if (MoveInput > 0.1f)
                LookDirection = Orientation.Right;
            else if (MoveInput < -0.1f)
                LookDirection = Orientation.Left;
        }

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

    bool JumpInput;
    bool JumpInputDown;
    void OnJump(InputValue value)
    {
        JumpInput = value.isPressed;
        JumpInputDown = value.isPressed;
    }

    void Jump()
    {
        if (JumpsLeft <= 0)
            return;

        JumpsLeft--;

        CurrentState = State.Jumping;

        VerticalVelocity = JumpStrength;
        JumpBoostTimeLeft = JumpBoostTime;
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

    bool DashInputDown;
    void OnDash(InputValue value) => DashInputDown = value.isPressed;

    void Dash()
    {
        if (DashesLeft <= 0 || DashCooldownLeft > 0.0f)
            return;

        DashDirection = LookDirection;

        // Backdash on ground without input.
        if (IsGrounded && Mathf.Abs(MoveInput) <= 0.1f)
            DashDirection = InvertOrientation(LookDirection);

        CurrentState = State.Dashing;

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
            MoveVelocity = (DashDirection == Orientation.Right ? 1.0f : -1.0f) * DashSpeed;
    }

    //////////////////////////////////////////////////////////////////////////

    [Space]

    public float CoyoteTime;
    float CoyoteTimeLeft;

    void UpdateCoyoteTime()
    {
        if (IsGrounded)
            CoyoteTimeLeft = CoyoteTime;
        else
            CoyoteTimeLeft -= Time.fixedDeltaTime;
    }

    //////////////////////////////////////////////////////////////////////////

    void ResetInputDown()
    {
        JumpInputDown = false;
        DashInputDown = false;
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
        UpdateCoyoteTime();
        UpdateMoveVelocity();
        UpdateDash();

        if (JumpInputDown)
            Jump();

        if (DashInputDown)
            Dash();

        if (CurrentState == State.Grounded)
        {
            // Always reset jumps when grounded.
            JumpsLeft = Jumps;

            // Add slight downward velocity to prevent peter-panning.
            VerticalVelocity = -0.1f;
        }
        else if (CurrentState == State.Jumping)
        {
            // Holding jump for longer makes you go higher.

            if (JumpInput)
                JumpBoostTimeLeft -= Time.fixedDeltaTime;
            else
                JumpBoostTimeLeft = 0.0f;

            if (JumpBoostTimeLeft > 0.0f)
                VerticalVelocity += JumpBoostStrength * Time.fixedDeltaTime;
            else
                VerticalVelocity -= Gravity * Time.fixedDeltaTime;
        }
        else if (CurrentState == State.Falling)
        {
            // We loose one jump when falling (and coyote-time has passed).
            if (CoyoteTimeLeft < 0.0f && JumpsLeft == Jumps)
                JumpsLeft--;

            VerticalVelocity -= Gravity * Time.fixedDeltaTime;
        }

        VerticalVelocity = Mathf.Max(VerticalVelocity, -MaxFallSpeed);

        RigidBody.velocity = new Vector2(MoveVelocity, VerticalVelocity);

        ResetInputDown();
    }
}
