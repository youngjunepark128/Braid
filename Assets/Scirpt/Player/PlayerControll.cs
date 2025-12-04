using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private const string RUN =  "Run";
    private const string JUMP = "Jump";
    private static readonly int DESCENDING = Animator.StringToHash("Descending");
    private static readonly int ASCENDING = Animator.StringToHash("Ascending");    
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int DESCENT = Animator.StringToHash("Descent");
    
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

    private enum JumpState
    {
        Grounded,
        Ascending,
        Descending,
        Descent,
    }
    
    private JumpState jumpState = JumpState.Grounded;
    private bool isGrounded = true;
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
        // 기존 좌우 이동 코드... (생략)

        // 4. 점프 힘 적용
        if (jumpRequest)
        {
            // Y축 속도를 강제로 jumpForce로 설정 (힘을 더하는 AddForce보다 반응이 빠름)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpRequest = false; // 점프 처리 완료
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundChecker == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
    }
}
