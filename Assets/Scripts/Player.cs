using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Attack details")]
    public Vector2[] attackMovement;

    public bool IsBusy { get; private set; }        // 判断当前是否处于闲置状态

    [Header("Move info")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashCoolDown;    // 技能CD
    private float dashUseageTimer;                  // 技能可使用剩余时间
    public float dashSpeed;
    public float dashDuration;
    public float DashDir { get; private set; }

    [Header("Collision info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    public int FacingDir { get; private set; } = 1;
    private bool facingRight = true;

    #region Components
    public Animator Anim { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    #endregion

    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerPrimaryAttackState PrimaryAttack { get; private set; }
    #endregion

    public void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState = new PlayerAirState(this, StateMachine, "Jump");
        DashState = new PlayerDashState(this, StateMachine, "Dash");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "WallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, "Jump");
        PrimaryAttack = new PlayerPrimaryAttackState(this, StateMachine, "Attack");
    }

    public void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();

        CheckForDashInput();
    }

    // 使用携程来保持游戏的流畅性
    public IEnumerator BusyFor(float seconds)
    {
        IsBusy = true;                                  // 标记协程正在执行中。
        yield return new WaitForSeconds(seconds);       // 用于暂停协程的执行, yield关键字用于暂停并返回当前的值，这里是WaitForSeconds对象。
        IsBusy = false;                                 // 表示协程已经完成等待，可以继续执行。
    }

    // 攻击动画触发器
    public void AnimationTrigger() => this.StateMachine.CurrentState.AnimationFinishTrigger();

    // 检测玩家冲刺输入
    private void CheckForDashInput()
    {
        if (this.IsWallDetetected())    // 检测到墙体时就不进入冲刺
        {
            return;
        }

        dashUseageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUseageTimer < 0)
        {
            dashUseageTimer = dashCoolDown;
            DashDir = Input.GetAxisRaw("Horizontal");

            if (DashDir == 0)
            {
                DashDir = FacingDir;
            }

            this.StateMachine.ChangeState(DashState);
        }
    }

    #region Velocity
    // 速度设置为0 (停止移动)
    public void ZeroVelocity() => Rb.velocity = new Vector2(0, 0);

    // 角色速度设置
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Rb.velocity = new Vector2(xVelocity, yVelocity);
        FilpController(xVelocity);
    }
    #endregion

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
    #endregion

    #region Filp
    // 角色翻转
    public void Filp()
    {
        FacingDir = FacingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    // 角色翻转控制器
    public void FilpController(float x)
    {
        if (x > 0 && !facingRight)
        {
            Filp();
        }
        else if (x < 0 && facingRight)
        {
            Filp();
        }
    }
    #endregion
}


