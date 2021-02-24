using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //MovementControll parameter
    public bool gameStart;
    private Rigidbody2D rigidbody2D;
    private Collider2D bodyCollider2D;
    private Collider2D headCollider2D;
    private Animator animator;

    public float speed = 5.0f;
    public float jumpForce=10.0f;
    public Transform groundCheck;
    public Transform ceilingCheck;

    public LayerMask groundLayer;

    private bool isGround, isJump;
    private bool jumpPressed;
    private bool crouchPressed;
    private int jumpCount;

    private float horizontalInput;
    //GameManager
    private GameManager gameManager;

    //Audio items
    public AudioClip hurtClip;
    public AudioClip addPointClip;
    public AudioClip jumpClip;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameStart = false;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider2D = GetComponent<CircleCollider2D>();
        headCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        AddEventListener();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (gameStart) {
            if (Input.GetButtonDown("Cancel"))
            {
                gameStart = !gameStart;
                EventManager.GetEventManager.TriggerEvent(EventEum.GamePause.ToString());
                return;
            }
            if (Input.GetButtonDown("Jump") && jumpCount > 0)
            {
                jumpPressed = true;
            }
            if (Input.GetButtonUp("SpeedUp"))
                speed = 5.0f;
            if (Input.GetButtonDown("SpeedUp")) {
                speed *= 2;
            }

            if (!Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, groundLayer))
                if (Input.GetButton("Crouch"))
                {
                    crouchPressed = true;
                    headCollider2D.enabled = false;
                    rigidbody2D.gravityScale = 3.0f;

                }
                else
                {
                    crouchPressed = false;
                    headCollider2D.enabled = true;
                    rigidbody2D.gravityScale = 1.0f;
                }
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position,0.1f,groundLayer);
        if (gameStart) {
            GroundMovement();
            JumpMovement();
        }
        AnimateSwitch();
    }


    void GroundMovement() {

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rigidbody2D.velocity = new Vector2(horizontalInput*speed,rigidbody2D.velocity.y);

        if (horizontalInput != 0) {
            transform.localScale=new Vector3(horizontalInput,1,1);
        }
    }

    void JumpMovement() {
        if (isGround) {
            jumpCount = 2;
            isJump = false;

        }
        if (jumpPressed)
        {
            isJump = true;
            audioSource.PlayOneShot(jumpClip);
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        if (rigidbody2D.velocity.y < 0)
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * 3.0f * Time.fixedDeltaTime;
        else if (rigidbody2D.velocity.y > 0 && Input.GetAxisRaw("Jump") != 1)//跳跃时如果玩家松开跳跃键则y方向上减速
            rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.fixedDeltaTime;
    }

    void AnimateSwitch() {

        animator.SetFloat("Running",Mathf.Abs(rigidbody2D.velocity.x));
        if (isGround)
            animator.SetBool("Falling", false);
        else
            if (!isGround && rigidbody2D.velocity.y > 0&&isJump){
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling",false);
        }
        else
            if (rigidbody2D.velocity.y < 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
            
        }
        animator.SetBool("Crouching",crouchPressed);

    }

    //Event listener

    void AddEventListener() {
        EventManager.GetEventManager.StartListening(EventEum.GameStart.ToString(), () => {
            gameStart = true;
        });
        EventManager.GetEventManager.StartListening(EventEum.GamePause.ToString(), ()=> {
            gameStart = false;
        });
    }

    void GameStart() {
        gameStart = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cherry"))
        {
            audioSource.PlayOneShot(addPointClip);
            jumpCount++;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("gem"))
        {
            audioSource.PlayOneShot(addPointClip);
            gameManager.UpDateScore(1);
            Destroy(collision.gameObject);
        }

        //Fall
        if (collision.CompareTag("deadLine"))
        {
            EventManager.GetEventManager.TriggerEvent(EventEum.PlayerDead.ToString());
            rigidbody2D.velocity = new Vector2(0,0);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy")) {
            if (animator.GetBool("Falling"))
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger("Death");
                collision.gameObject.GetComponent<AudioSource>().Play();
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            }
            else
            {
                animator.SetBool("Hurt",true);
                audioSource.PlayOneShot(hurtClip);
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            }
        }
    }

    private void HurtReturn() {
        animator.SetBool("Hurt",false);
        animator.SetBool("Jumping", false);
        animator.SetBool("Falling", true);
    }
}
