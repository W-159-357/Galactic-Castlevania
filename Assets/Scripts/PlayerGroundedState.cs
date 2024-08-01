using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            player.StateMachine.ChangeState(player.PrimaryAttack);
        }

        if (!player.IsGroundDetected())
        {
            player.StateMachine.ChangeState(player.AirState);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && player.IsGroundDetected())
        {
            player.StateMachine.ChangeState(player.JumpState);
        }
    }
}
