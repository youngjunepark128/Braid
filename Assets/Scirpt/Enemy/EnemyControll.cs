using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")] 
    private Animator animator { get; set; }
    private SpriteRenderer spriteRenderer {get; set;}
    private  Rigidbody2D rigidBody { get; set; }

    
    [Header("Components")]
    //[field: SerializeField] private Rigidbody2D rb { get; set; }
    [field: SerializeField] private Transform GroundChecker;
    [field: SerializeField] private float GroundCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask GroundLayerMask { get; set; }
    [field: SerializeField] private Transform WallChecker;
    [field: SerializeField] private float WallCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask WallLayerMask { get; set; }
    [field : SerializeField] private CapsuleCollider2D coll;
    public float walkSpeed =3.0f;
    private bool jumpRequest = false;    
    
    private bool IsWalled = false;
    private bool IsAttacked = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (IsAttacked) return;
        CheckIfGrounded();
        CheckIfWalled();
        Move();
        Jump();
        if(IsWalled) Flip();
    }

    
    private void Move()
    {
        transform.position += Vector3.right*walkSpeed*Time.deltaTime;
    }
    private void Flip()
    {
        // 오른쪽 반전
        if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            walkSpeed *= -1;
        }
        // 왼쪽 반전
        else if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            walkSpeed *= -1;
            IsWalled = false;
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
    private static readonly int IS_WALLED = Animator.StringToHash("IsWalled");
    private static readonly int IS_ATTACKED = Animator.StringToHash("IsAttacked");
    
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
    //--------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControll player = collision.gameObject.GetComponent<PlayerControll>();
            if (player.VerticalSpeed <= 0 && player.transform.position.y > transform.position.y + 0.3f)
            {
                player.Bounce();
                Die();
            }
            else
            {
                player.TakeDamage();
            }
        }
    }

    void Die()
    {
        animator.SetBool(IS_ATTACKED, true);
        IsAttacked = true;
        coll.enabled = false;
        Vector3 scaler = transform.localScale;
        scaler.y *= -1; 
        transform.localScale = scaler;
        StartCoroutine(DeadFallSequence());
    }
    
    IEnumerator DeadFallSequence()
    {
        float timer = 0f;
        float duration = 3.0f; // 3초 동안 떨어짐 (이후엔 화면 밖일 테니)
        
        // 초기 속도 설정 (위로 살짝 튐)
        float verticalVelocity = 5.0f; 

        while (timer < duration)
        {
            // 되감기 중이라면 코루틴 강제 중단 (시스템에 맡김)
            if (TimeManager.isRewinding) yield break;

            // 물리 공식: v = v0 + at
            verticalVelocity += gravity * Time.deltaTime;
            
            // 위치 이동: y = y + vt
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
    }
    //---------------------------------------------------
    private void CheckIfGrounded()
    {
       
        prevIsGrounded = isGrounded; 
        isGrounded = Physics2D.OverlapCircle(GroundChecker.position, GroundCheckRadius, GroundLayerMask);
        animator.SetBool(IS_GROUNDED, isGrounded);
    }

    private void CheckIfWalled()
    {
        IsWalled = Physics2D.OverlapCircle(WallChecker.position, WallCheckRadius, WallLayerMask);
        animator.SetBool(IS_WALLED, IsWalled);
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundChecker == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
        
        if (WallChecker == null) return;
        Gizmos.color = Color.yellowGreen;
        Gizmos.DrawWireSphere(WallChecker.position, WallCheckRadius);
    }
}
