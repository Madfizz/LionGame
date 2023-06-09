using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionIdleState : PlayerState
{
    float _inputBufferTime = 0.1f;
    float _inputBufferTimer;
    bool _isFalling;

    public LionIdleState(Player player) : base(player) {}

    public override void Enter()
    {
        base.Enter();
        _isFalling = false;
        Player.Rb.velocity = Vector3.zero;
        ChangeForms(FormType.Lion);
        _inputBufferTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        IsFalling();

        // Jump 
        if (Input.GetButtonDown("Jump") && Player.IsGrounded)
        {
            // Change to idle jump
            Player.StateMachine.ChangeState(Player.L_IdleJumpState);
        }
        // Move
        else if (Player.XInput != 0 && _inputBufferTimer > _inputBufferTime)
        {
            Player.StateMachine.ChangeState(Player.L_MoveState);
        }
        // If player releases movement input, reset input buffer time
        else if (Player.XInput == 0)
        {
            _inputBufferTimer = 0;
        }
        // Increment buffer timer
        else 
        {
            _inputBufferTimer += Time.deltaTime;
        }
    }

    public override void PhysicsUpdate() {}

    void IsFalling()
    {
        if (Player.Rb.velocity.y < -1f && !_isFalling && !GroundCheck())
        {
            _isFalling = true;
            ChangeForms(FormType.DownJump);
        }
        else if (Player.Rb.velocity.y >= -1f && _isFalling && GroundCheck()) 
        {
            _isFalling = false;
            ChangeForms(FormType.Lion);
        }
    }
}
