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
        //rb = GetComponent<Rigidbody2D>();
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
    }

    private void Move()
    {
        float moveAbs = Mathf.Abs(moveInput);
        animator.SetFloat(RUN,moveAbs);
        
        transform.position += Vector3.right*moveInput*moveSpeed;
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
    
    [Header("Gravity Settings")]
    public float gravity = 3.0f;
    public float currentGravity;
    
    [Header("Jump Settings")]
    public float VerticalSpeed;
    public float jumpForce = 0.0f;

    private void Jump()
    {
        if(isGrounded) currentGravity = 0.0f;
        if (jumpRequest)
        {
            jumpForce = 15.0f;
            jumpRequest = false; 
        }
        currentGravity += gravity * Time.deltaTime;
        VerticalSpeed = jumpForce - currentGravity;
        animator.SetFloat("VerticalSpeed", VerticalSpeed);
        
        MoveY(VerticalSpeed);
        
        void MoveY(float ySpeed)
        {
            float newY = transform.position.y + (ySpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        if (GroundChecker == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
    }
}
