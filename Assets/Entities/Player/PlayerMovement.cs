using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

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

    void UpdateCurrentState()
    {
        if (Physics2D.OverlapPoint(Feet.transform.position, LayerMask.GetMask("Terrain")))
            CurrentState = State.Grounded;
        else if (VerticalVelocity > 0.0f)
            CurrentState = State.Jumping;
        else
            CurrentState = State.Falling;
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
    public float JumpBoostTime;
    float JumpBoostTimeLeft;

    bool JumpInput;
    bool JumpInputDown;
    void OnJump(InputValue value)
    {
        JumpInput = value.isPressed;
        JumpInputDown = value.isPressed;
    }

    void ResetJumpInputUpDown()
    {
        JumpInputDown = false;
    }

    void Jump()
    {
        if (JumpsLeft > 0)
        {
            JumpsLeft--;
            VerticalVelocity = JumpStrength;
            JumpBoostTimeLeft = JumpBoostTime;
        }
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

    public GameObject Feet;

    Rigidbody2D RigidBody;

    void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(RigidBody);
    }

    void FixedUpdate()
    {
        UpdateCurrentState();
        UpdateCoyoteTime();
        UpdateMoveVelocity();

        if (CurrentState == State.Grounded)
        {
            if (JumpInputDown)
            {
                Jump();
            }
            else
            {
                JumpsLeft = Jumps;

                // add slight downward velocity to prevent peter-panning
                VerticalVelocity = -0.1f;
            }
        }
        else if (CurrentState == State.Jumping)
        {
            if (JumpInputDown)
                Jump();

            if (JumpInput)
                JumpBoostTimeLeft -= Time.fixedDeltaTime;
            else
                JumpBoostTimeLeft = 0.0f;

            if (JumpBoostTimeLeft <= 0.0f)
                VerticalVelocity -= Gravity * Time.fixedDeltaTime;
        }
        else if (CurrentState == State.Falling)
        {
            // We lose one jump when falling (and coyote-time has passed).
            if (CoyoteTimeLeft < 0.0f && JumpsLeft == Jumps)
                JumpsLeft--;

            if (JumpInputDown)
                Jump();
            else
                VerticalVelocity -= Gravity * Time.fixedDeltaTime;
        }

        RigidBody.MovePosition(RigidBody.position + new Vector2(MoveVelocity, VerticalVelocity) * Time.fixedDeltaTime);

        ResetJumpInputUpDown();
    }
}
