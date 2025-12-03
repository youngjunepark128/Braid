using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private const string DESCENDING = "Descending";
    private const string ASCENDING = "Ascending";
    private const string RUN =  "Run";
    private const string START_ASCENDING = "Start Ascending";
    private const string START_DESCENDING = "Start Descending";

    private Animator animator { get; set; }
    private SpriteRenderer spriteRenderer {get; set;}

    [Header("Settings")] 
    public float moveSpeed = 3.0f;
    public float jumpForce = 7.0f;

    [Header("Jump Settings")]
    public float jumpHeight = 0.0f;
    public float ascendingSpeed = 1.0f;
    public float descendingSpeed = 1.0f;

    [field: SerializeField] private Transform GroundChecker;
    [field: SerializeField] private float GroundCheckRadius = 0.1f;
    [field: SerializeField] private LayerMask GroundLayerMask { get; set; }
    private float moveInput;
    
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        Move();
        Flip();
    }

    private void Move()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float absMovement = Mathf.Abs(inputVector.magnitude);
        animator.SetFloat(RUN,absMovement);
        
        Vector2 moveVector = new Vector2(inputVector.x, 0.0f);
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
        Descending
    }
    
    private bool isGrounded = true;
    private bool prevIsGrounded;
    private void Jump()
    {
        
    }

    private void Ascending()
    {
        if (jumpHeight > 2.0f)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }

    private void Descending()
    {
        
    }

    private void GroundCheck()
    {
        
    }
    private void CheckIfGrounded()
    {
        //Physics2D : https://developerkoohoo.web.app/Unity/UnityBasic/12.%20Physics2D/
        
        // 2D에서 가장 많이 사용하는 Physics2D 함수
        // Physics2D.Raycast(); Physics2D.RaycastAll(); 
        // : 선과 겹쳐진(충돌한) 콜라이더가 있는지 검사합니다.
        // Physics2D.OverlapBox(); Physics2D.OverlapBoxAll();
        // : 박스(사각형)와 겹쳐진(충돌한) 콜라이더가 있는지 검사합니다.
        // Physics2D.OverlapCircle(); Physics2D.OverlapCircleAll();
        // : 원과 겹쳐진(충돌한) 콜라이더가 있는지 검사합니다.

        prevIsGrounded = isGrounded; //isGrounded를 갱신하기 전에 prev에 보관한다
        isGrounded = Physics2D.OverlapCircle(GroundChecker.position, GroundCheckRadius, GroundLayerMask);
        
        animator.SetBool(IS_GROUNDED, isGrounded);
        
    }
    
}
