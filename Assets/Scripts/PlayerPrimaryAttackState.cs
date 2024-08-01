using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;           // 组合攻击计数器
    private float lastTimeAttacked;     // 最后攻击时间
    private float comboWindow = 2;      // 组合攻击持续时间

    public PlayerPrimaryAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        player.Anim.SetInteger("ComboCounter", comboCounter);

        float attackDir = player.FacingDir;     // 攻击方向默认为玩家朝向
        if (xInput != 0)
        {
            attackDir = xInput;                 // 攻击方法随着移动方向而变化
        }
        // player.Anim.speed = 1.2f;       // 设置攻击速度

        // 实现出可以攻击的移动效果
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);
        stateTime = .1f;    // 使用状态定时器实现类似于惯性的效果

    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        // player.Anim.speed = 1;          // 退出攻击时，动画速度回复正常
        comboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTime < 0)
        {
            player.ZeroVelocity();
        }
        if (triggerCalled)
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
}
