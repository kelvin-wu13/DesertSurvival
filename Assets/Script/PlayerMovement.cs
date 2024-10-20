using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller; // Reference to the CharacterController component
    public float speed = 12f;              // Player's movement speed
    public float gravity = -9.81f;         // Gravity force
    public float jumpHeight = 3f;          // Jump height

    public Transform groundCheck;          // Reference to an empty object used to check if the player is on the ground
    public float groundDistance = 0.4f;    // Radius of the sphere to detect the ground
    public LayerMask groundMask;           // LayerMask to identify what objects are considered ground

    private HealthSystem healthSystem;

    Vector3 velocity;                      // Player's velocity (for gravity and jump)
    bool isGrounded;                       // Whether the player is currently grounded

    void Update()
    {
        // Check if the player is grounded by casting a small sphere at the groundCheck position
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // If the player is grounded and moving downwards, reset the downward velocity (stop falling)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Keep a small downward velocity to ensure proper ground detection
        }

        // Get input for movement (WASD or arrow keys)
        float x = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow
        float z = Input.GetAxis("Vertical");   // W/S or Up/Down arrow

        // Move the player based on input
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Apply a jump force based on the jumpHeight formula
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;

        // Move the player downwards based on gravity
        controller.Move(velocity * Time.deltaTime);
    }
    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }
}