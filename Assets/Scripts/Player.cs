using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    #endregion

    public void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState = new PlayerAirState(this, StateMachine, "Jump");
        DashState = new PlayerDashState(this, StateMachine, "Dash");
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

    private void CheckForDashInput()
    {
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

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Rb.velocity = new Vector2(xVelocity, yVelocity);
        FilpController(xVelocity);
    }

    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

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
}


