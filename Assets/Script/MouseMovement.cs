using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity for the mouse movement
    public Transform playerBody;          // Reference to the player's body for horizontal rotation (left/right)
    
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input for both X (horizontal) and Y (vertical) axes
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        // Rotate the player's body horizontally (left/right) based on mouse X movement
        playerBody.Rotate(Vector3.up * mouseX);

        // Apply vertical rotation (up/down) based on mouse Y movement
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical rotation to prevent flipping

        yRotation += mouseX;
        // Rotate the camera vertically (up/down)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}