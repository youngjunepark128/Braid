using System;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    private const string PLAYER_ASCENDING = "Player Ascending";
    private const string PLAYER_DESCENDING = "Player Descending";
    private const string PLAYER_RUN =  "Player Run";
    private const string PLAYER_START_ASCENDING = "Player Start Ascending";
    private const string PLAYER_START_DESCENDING = "Player Start Descending";

    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;

    [Header("Settings")] 
    public float moveSpeed = 3.0f;
    public float jumpForce = 7.0f;

    private float moveInput;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput =  Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        animator.SetFloat(PLAYER_RUN,Mathf.Abs(moveInput));
        
        
    }
    
}
