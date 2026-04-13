using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeedNormal = 5.0f;
    public float moveSpeedSneak = 2.0f;
    public float gravity = -9.81f;

    public InputActionReference moveInput;
    public Transform playerCamera;

    CharacterController controller;
    PlayerInput playerInput;
    Vector3 velocity;
    bool isSneaking;
    public TextMeshProUGUI playerStateText;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void Update()
    {
        PlayerMotion();

        // Debug reset button
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void PlayerMotion()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 moveDirection = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
        //Vector3 move = playerCamera.transform.right * moveDirection.x + playerCamera.transform.forward * moveDirection.y;
        Vector3 move = Vector3.right * moveDirection.x + Vector3.forward * moveDirection.y;
        Vector3 moveVelocity;

        // Change speed if the player is holding the sneak button
        isSneaking = playerInput.currentActionMap["Sneak"].IsPressed();
        if (isSneaking)
        {
            moveVelocity = move * moveSpeedSneak;
            playerStateText.text = "Sneaking";
        }
        else
        {
            moveVelocity = move * moveSpeedNormal;
            playerStateText.text = "Walking";
        }

        velocity.y += gravity * Time.deltaTime;

        moveVelocity.y = velocity.y;

        controller.Move(moveVelocity * Time.deltaTime);


        Vector3 horizontalVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
    }
}