using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.ZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (xInput == player.FacingDir && player.IsWallDetetected())
            return;

        if (xInput != 0 && !player.IsBusy)     // // 输入不为0，就切换到移动状态
        {
            player.StateMachine.ChangeState(player.MoveState);
        }
    }
}
