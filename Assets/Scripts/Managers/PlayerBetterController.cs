using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBetterController : MonoBehaviour
{
    public bool gameStart;

    [Header ("HorizontalMoveParam")]
    public Vector2 playerDir;
    public float WalkSpeed;
    public float SpeedMult;
    public float AccelerateTime;
    public float DecelerateTime;
    public float horizontalInput;
    private bool CanMove = true;

    [Header("OnGroundTestParam")]
    public Vector2 ColliderSize;
    public Vector2 ColliderOffset;
    public LayerMask GroundLayerMask;
    public bool IsOnGround;

    [Header("JumpingParam")]
    public int JumpCount;
    private int curJumpCount;
    public bool JumpPressed;
    public float JumpingSpeed;
    //gravity modifier
    public float FallMultiplier;
    public float LowJumpMultiplier;
    private bool CanJump=true;
    private bool GravityModifier = true;

    [Header("DashingParam")]
    public float DashSpeed;
    public int DashCount;
    private int curDashCount;
    public bool DashPressed;
    private bool WasDash=false;
    // Dashing Drag Param
    public float DragMaxForce;
    public float DragDuration;
    public float DashWaitTime;


    [Header("ClimbWallParam")]
    public Vector2 WallJumpingSpeed;
    public float GrabWallMaxTime;
    private Coroutine climbWallCoroutine;
    public bool CanClimbWall = true;
    public bool Loose=false;
    //Wall Collision test
    public Vector2 WallColliderSize;
    public Vector2 WallColliderOffset;
    public LayerMask WallLayerMask;
    public bool IsOnWall;
    [Header("AnimateParam")]
    //Animate switch
    public bool IsJumping;
    public bool IsDashing;


    [Header("AudioParam")]
    public AudioClip addPointClip;
    public AudioClip hurtClip;
    public AudioClip jumpClip;
    private AudioSource audioSource;
    

    private GameManager gameManager;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float velocityX;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        curJumpCount = JumpCount;
        curDashCount = DashCount;
        AddEventListeners();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
            if (Input.GetButtonDown("Cancel"))
            {
                gameStart = !gameStart;
                EventManager.GetEventManager.TriggerEvent(EventEum.GamePause.ToString());
                return;
            }
        //JumpPressed = (Input.GetAxis("Jump") == 1);
        if (Input.GetButtonDown("Jump") && curJumpCount > 0)
            JumpPressed = true;
        if (Input.GetButtonDown("Dash") && curDashCount > 0)
            DashPressed = true;
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        //如果在地面上,重置运动状态
        IsOnGround = OnGround();
        if (IsOnGround)
        {
            curJumpCount = JumpCount;
            curDashCount = DashCount;
            IsJumping = false;
            Loose = false;
        }
        if (gameStart) {

            #region 移动
            if (CanMove)
            {
                if (horizontalInput > 0)
                {
                    rigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(rigidbody2D.velocity.x, WalkSpeed * Time.fixedDeltaTime * SpeedMult, ref velocityX, AccelerateTime), rigidbody2D.velocity.y);
                    playerDir = Vector2.right;
                    spriteRenderer.flipX = false;
                }
                else if (horizontalInput < 0)
                {
                    rigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(rigidbody2D.velocity.x, WalkSpeed * Time.fixedDeltaTime * SpeedMult * -1, ref velocityX, AccelerateTime), rigidbody2D.velocity.y);
                    playerDir = Vector2.left;
                    spriteRenderer.flipX = true;
                }
                else
                {
                    rigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(rigidbody2D.velocity.x, 0, ref velocityX, DecelerateTime), rigidbody2D.velocity.y);
                }
            }
            #endregion

            #region 跳跃

            if (CanJump)
            {
                //如果跳跃键按下(持续按住跳跃键无效)，跳跃次数大于0 则跳跃
                if (JumpPressed)
                {
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpingSpeed);
                    curJumpCount--;
                    JumpPressed = false;
                    audioSource.PlayOneShot(jumpClip);
                    IsJumping = true;
                    Loose = false;

                }

            }

            //重力调整
            if (GravityModifier)
            {
                if (rigidbody2D.velocity.y < 0)
                    rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * FallMultiplier * Time.fixedDeltaTime;
                else if (rigidbody2D.velocity.y > 0 && Input.GetAxisRaw("Jump") != 1)//跳跃时如果玩家松开跳跃键则y方向上减速
                    rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * LowJumpMultiplier * Time.fixedDeltaTime;
            }
            #endregion

            #region 冲刺
            if (DashPressed)
            {
                //清除玩家动量
                rigidbody2D.velocity = Vector2.zero;
                //施加一个力
                rigidbody2D.velocity += playerDir * DashSpeed;
                DashPressed = false;

                curDashCount--;
                StartCoroutine(Dash());
            }
            #endregion

            #region 爬墙


            if (IsOnWall)
            {
                curJumpCount = JumpCount;
                rigidbody2D.isKinematic = true;
            }
            else
            {
                rigidbody2D.isKinematic = false;
            }
            if (Loose)
            {
                rigidbody2D.velocity += Vector2.up * Physics2D.gravity * Time.fixedDeltaTime;
            }


            #endregion
        }
        AnimateSwitch();
    }

    IEnumerator Dash() {
        //关闭玩家移动和跳跃
        CanMove = false;
        CanJump = false;
        //关闭重力调整器
        GravityModifier = false;
        //关闭重力影响
        rigidbody2D.gravityScale = 0;
        //施加空气阻力 rigidbody.drag
        DOVirtual.Float(DragMaxForce,0, DragDuration,(float input)=> {
            rigidbody2D.drag = input;
        });
        WasDash = true;
        //等待一段时间
        yield return new WaitForSeconds(DashWaitTime);
        //重新开启移动和跳跃，重力和调整器
        CanMove = true;
        CanJump = true;
        GravityModifier = true; 
        rigidbody2D.gravityScale = 1;
        WasDash = false;
    }

    IEnumerator ClimbWall() {
        yield return new WaitForSeconds(GrabWallMaxTime);
        Loose = true;
        
    }


    void AnimateSwitch() {
        animator.SetFloat("Running", Mathf.Abs(rigidbody2D.velocity.x));
        if (IsOnGround)
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
        }
        else
            if (rigidbody2D.velocity.y > 0 && IsJumping)
        {
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
        }
        else
            if (rigidbody2D.velocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
        }
        if (IsOnWall)
        {
            if (horizontalInput!=0)
                animator.speed = 1.0f;
            else
                animator.speed = 0.0f;
            animator.SetBool("ClimbWall", IsOnWall && !Loose);
            
            animator.SetBool("Falling", Loose||WasDash);
        }
        else
        {
            animator.SetBool("ClimbWall", false);
            animator.speed = 1.0f;
        }
        

        //animator.SetBool("Crouching", crouchPressed);
    }

    void AddEventListeners() {
        EventManager.GetEventManager.StartListening(EventEum.GameStart.ToString(),GameStart);
    }


    private bool OnGround() {
        return (Physics2D.OverlapBox((Vector2)transform.position + ColliderOffset, ColliderSize, 0, GroundLayerMask)!=null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Movement
        if (collision.CompareTag("wall"))
        {
            IsOnWall = true;
            climbWallCoroutine = StartCoroutine(ClimbWall());
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);
        }

        //GamePlay
        if (collision.CompareTag("cherry"))
        {
            audioSource.PlayOneShot(addPointClip);
            curJumpCount++;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("gem"))
        {
            audioSource.PlayOneShot(addPointClip);
            curDashCount++;
            gameManager.UpDateScore(1);
            Destroy(collision.gameObject);
        }

        //Fall
        if (collision.CompareTag("deadLine"))
        {
            EventManager.GetEventManager.TriggerEvent(EventEum.PlayerDead.ToString());
            audioSource.PlayOneShot(hurtClip);
            rigidbody2D.velocity = new Vector2(0, 0);
        }



    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
        {
            
            IsOnWall = false;
            StopCoroutine(climbWallCoroutine);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            if (animator.GetBool("Falling"))
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger("Death");
                collision.gameObject.GetComponent<AudioSource>().Play();
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpingSpeed);
            }
            else
            {
                animator.SetBool("Hurt", true);
                audioSource.PlayOneShot(hurtClip);
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpingSpeed);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + ColliderOffset, ColliderSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + WallColliderOffset, WallColliderSize);
    }



    void GameStart()
    {
        gameStart = true;
    }


}
