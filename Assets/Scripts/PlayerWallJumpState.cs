using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTime = 1f;
        player.SetVelocity(5 * -player.FacingDir, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTime < 0)
        {
            player.StateMachine.ChangeState(player.AirState);
        }

        if (player.IsGroundDetected())
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
}
