using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private const string RUN =  "Run";
    private const string JUMP = "Jump";
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    
    private Animator animator { get; set; }
    private SpriteRenderer spriteRenderer {get; set;}

    [Header("Settings")] 
    public float moveSpeed = 3.0f;
    
    [Header("Jump Settings")]
    public float jumpHeight = 5.0f;
    public float ascendingSpeed = 2.0f;
    public float descendingSpeed = 2.0f;
    
    
    
    [Header("Components")]
    [field: SerializeField] private Rigidbody2D rb { get; set; }
    [field: SerializeField] private Transform GroundChecker;
    [field: SerializeField] private float GroundCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask GroundLayerMask { get; set; }
    private float moveInput;
    private bool jumpRequest = false;    

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        Move();
        Jump();
        Flip();
        
        isGrounded = Physics2D.OverlapCircle(GroundChecker.position, GroundCheckRadius, IS_GROUNDED);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void Move()
    {
        float moveAbs = Mathf.Abs(moveInput);
        animator.SetFloat(RUN,moveAbs);
        
        Vector2 moveVector = new Vector2(moveInput, 0.0f);
        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
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
    
    private bool isGrounded;
    private bool prevIsGrounded;
    private Vector3 startPosition;
    
    private void Jump()
    {
        
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        
        void MoveY(float ySpeed)
        {
            float newY = transform.position.y + (ySpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        
    }
   

    void FixedUpdate() // 물리 연산 적용 (일정한 주기)
    {
        if (jumpRequest)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpRequest = false; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundChecker == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
    }
}
