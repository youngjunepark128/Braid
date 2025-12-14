using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private const string RUN =  "Run";
    private const string JUMP = "Jump";
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int WAIT_TIME = Animator.StringToHash("WaitTime");
    
    private Animator animator { get; set; }
    private SpriteRenderer spriteRenderer {get; set;}
    private  Rigidbody2D rigidBody { get; set; }
    private TakeDamage takeDamage;

    [Header("Settings")] 
    public float moveSpeed = 3.0f;
    public float waitTime = 0.0f;
    
    [Header("Components")]
    //[field: SerializeField] private Rigidbody2D rb { get; set; }
    [field: SerializeField] private Transform GroundChecker;
    [field: SerializeField] private float GroundCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask GroundLayerMask { get; set; }
    private float moveInput;
    private bool jumpRequest = false;    

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        takeDamage = GetComponent<TakeDamage>();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (TimeManager.isRewinding == true) return;
        if(takeDamage.IsDead == true) return;
        moveInput = Input.GetAxisRaw("Horizontal");
        CheckIfGrounded();
        Move();
        Jump();
        Flip();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }
        waitTime += Time.deltaTime;
        animator.SetFloat(WAIT_TIME, waitTime);
    }

    private void Move()
    {
        float moveAbs = Mathf.Abs(moveInput);
        animator.SetFloat(RUN,moveAbs);
        transform.position += Vector3.right*moveInput*moveSpeed*Time.deltaTime;
    }
    private void Flip()
    {
        // 오른쪽 이동
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // 왼쪽 이동
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    //-------------------------------------------
    // PlayerControll.cs 내용 중 추가

    public void Bounce()
    {
        VerticalSpeed = jumpForce * 0.7f; 
        animator.SetTrigger(JUMP); 
        isGrounded = false;
    }
    public void TakeDamage()
    {
        takeDamage.Die();
        Debug.Log("플레이어 사망!");
    }
    
    //-------------------------------------------
    
    private bool isGrounded;
    private bool prevIsGrounded;
    private Vector3 startPosition;
    
    [Header("Gravity Settings")]
    public float gravity = 3.0f;
    
    [Header("Jump Settings")]
    public float VerticalSpeed;
    public float jumpForce = 15.0f;

    private void Jump()
    {
        if (isGrounded && VerticalSpeed < 14.9f) VerticalSpeed = 0.0f;
        if (jumpRequest)
        {
            VerticalSpeed = jumpForce;
            jumpRequest = false;
            animator.SetTrigger(JUMP);
        }

        if (isGrounded == false || VerticalSpeed > 0)
        {
            VerticalSpeed -=  gravity * Time.deltaTime;
        }
        
        animator.SetFloat("VerticalSpeed", VerticalSpeed);
        MoveY(VerticalSpeed);
        
        
        
        void MoveY(float ySpeed)
        {
            float newY = transform.position.y + (ySpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    private void CheckIfGrounded()
    {
       
        prevIsGrounded = isGrounded; 
        isGrounded = Physics2D.OverlapCircle(GroundChecker.position, GroundCheckRadius, GroundLayerMask);
        animator.SetBool(IS_GROUNDED, isGrounded);
        
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundChecker == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
    }
}
