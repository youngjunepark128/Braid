using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float moveSpeed { get; set; }
    private Animator animator { get; set; }
    private SpriteRenderer spriteRenderer {get; set;}
    private  Rigidbody2D rigidBody { get; set; }

    
    [Header("Components")]
    //[field: SerializeField] private Rigidbody2D rb { get; set; }
    [field: SerializeField] private Transform GroundChecker;
    [field: SerializeField] private float GroundCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask GroundLayerMask { get; set; }
    private float moveInput;
    private bool jumpRequest = false;    
    
    private bool IsWalled = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckIfGrounded();
        Move();
        Jump();
        if(IsWalled) Flip();
    }

    private void Move()
    {
        float moveAbs = Mathf.Abs(moveInput);
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
    
    private bool isGrounded;
    private bool prevIsGrounded;
    private Vector3 startPosition;
    
    [Header("Gravity Settings")]
    public float gravity = 3.0f;
    
    [Header("Jump Settings")]
    public float VerticalSpeed;
    public float jumpForce = 15.0f;

    private static readonly int ISJUMPED = Animator.StringToHash("IsJumping");
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    
    private void Jump()
    {
        if (isGrounded && VerticalSpeed < 14.9f) VerticalSpeed = 0.0f;
        if (jumpRequest)
        {
            VerticalSpeed = jumpForce;
            jumpRequest = false;
            animator.SetTrigger(ISJUMPED);
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
