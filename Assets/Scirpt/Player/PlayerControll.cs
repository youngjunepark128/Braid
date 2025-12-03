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
    Rigidbody2D rigidBody;

    [Header("Settings")] 
    public float moveSpeed = 3.0f;
    public float jumpForce = 7.0f;

    private float moveInput;
    

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

    private void Jump()
    {
        
    }

    private void Ascending()
    {
        
    }

    private void Descending()
    {
        
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
    // private void Flip()
    // {
    //     // 오른쪽 이동
    //     if (moveInput > 0)
    //     {
    //         transform.localScale = new Vector3(1, 1, 1);
    //     }
    //     // 왼쪽 이동
    //     else if (moveInput < 0)
    //     {
    //         transform.localScale = new Vector3(-1, 1, 1);
    //     }
    // }
    
}
