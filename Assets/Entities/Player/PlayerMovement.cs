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
    }

    State CurrentState = State.Grounded;
    bool IsGrounded => CurrentState == State.Grounded;

    public Collider2D Head;
    public Collider2D Feet;
    ContactFilter2D TerrainContactFilter;

    void UpdateCurrentState()
    {
        if (Physics2D.OverlapCollider(Feet, TerrainContactFilter, new Collider2D[1]) > 0)
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

    public float MoveSpeed;
    public float MoveResponsiveness;
    public float MoveResponsivenessInAir;
    float MoveVelocity;

    float MoveInput;
    void OnMove(InputValue value) => MoveInput = value.Get<float>();

    void UpdateMoveVelocity()
    {
        var responsiveness = MoveResponsiveness;
        if (!IsGrounded)
            responsiveness = MoveResponsivenessInAir;

        var newMoveVelocity = MoveSpeed * MoveInput;
        MoveVelocity -= responsiveness * (MoveVelocity - newMoveVelocity);
    }

    //////////////////////////////////////////////////////////////////////////

    public float Gravity;
    public float MaxFallSpeed;
    float VerticalVelocity;

    //////////////////////////////////////////////////////////////////////////

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

    void ResetJumpInputDown()
    {
        JumpInputDown = false;
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

        if (JumpInputDown)
            Jump();

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

        ResetJumpInputDown();
    }
}
