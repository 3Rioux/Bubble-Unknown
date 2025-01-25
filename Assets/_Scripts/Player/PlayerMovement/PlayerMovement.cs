using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Movement speed
    [SerializeField] private float jumpForce = 10f; // Jump force

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb; // Rigidbody2D component
    private PlayerInput playerInput; // PlayerInput component
    private InputAction moveAction; // Reference to the Move action
    private InputAction jumpAction; // Reference to the Jump action

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // Ground check position
    [SerializeField] private float groundCheckRadius = 0.2f; // Ground check radius
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground detection

    private Vector2 moveInput; // Store movement input
    private bool isGrounded; // Track if the player is on the ground

    private void Awake()
    {
        // Initialize references
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        //jumpAction = playerInput.actions["Jump"];

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        // Update input values
        moveInput = moveAction.ReadValue<Vector2>();

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle jump input
        //if (jumpAction.triggered && isGrounded)
        //{
        //    Jump();
        //}
    }

    private void FixedUpdate()
    {
        // Apply movement
        Move();
    }

    private void Move()
    {
        //// Horizontal movement
        //float moveX = moveInput.x * moveSpeed;
        //rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);

        //Move All Directions: 
        // Move the player based on input
        Vector2 movement = moveInput * moveSpeed;
        rb.linearVelocity = movement;

    }

    private void Jump()
    {
        // Apply vertical force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
