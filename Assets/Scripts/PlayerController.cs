using System;
using System.Collections;
using DefaultNamespace;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject persistentUI;
    public UnityEvent TUIInGame;

    [Header("Player")] private CapsuleCollider playerCollider;
    private Rigidbody rb;

    [Header("Move")] public float moveSpeed = 10f;

    public float groundDamping = 5f;

    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector3 movedirection;

    [Header("Jump")] public float jumpForce = 17f;
    public float jumpCooldown = 0.1f;
    public float airMultiplier = 0.4f;

    private bool isReadyToJump = true;

    [Header("Ground Check")] public float playerHeight = 2;
    public LayerMask groundLayer;

    private bool isGrounded;

    [Header("Camera")] private Vector2 cameraInput;
    public CinemachineCamera CineCamera;
    public CinemachineOrbitalFollow OrbitalCamera;
    private InputAxis horizontalAxis;
    private InputAxis verticalAxis;

    public float sensX;
    public float sensY;

    [Header("Object Atlas")] public Transform cameraOrientation;

    private void Awake()
    {
        if (UIManager.Instance == null)
        {
            Instantiate(persistentUI);
        }

        UIManager.Instance.TUIInGame.SetGameStartedFlag(true);
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerHeight = playerCollider.height;
    }

    private void Update()
    {
        GroundCheck();

        PlayerMove();
        ControlSpeedVelocity();

        ControlCamera();
    }

    private void FixedUpdate()
    {
        //Move
        if (isGrounded)
        {
            rb.AddForce(movedirection.normalized * (moveSpeed * 10), ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(movedirection.normalized * (moveSpeed * 10 * airMultiplier), ForceMode.Force);
        }
    }

    private void GroundCheck()
    {
        //Ground Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        if (isGrounded)
        {
            rb.linearDamping = groundDamping;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void PlayerMove()
    {
        //Move
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        cameraInput = playerInput.actions["Camera"].ReadValue<Vector2>();

        Vector3 flatForward = Vector3.ProjectOnPlane(cameraOrientation.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(cameraOrientation.right, Vector3.up).normalized;

        movedirection = flatForward * moveInput.y + flatRight * moveInput.x;
    }

    private void ControlSpeedVelocity()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
        }
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log($"isReadyToJump{isReadyToJump}");
            Debug.Log($"isGrounded{isGrounded}");

            if (isReadyToJump && isGrounded)
            {
                Debug.Log("condition key successful ");

                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                isReadyToJump = false;
                //TODO Take into account for BunnyJump Mechanic
                Invoke("ResetJump", jumpCooldown);
            }
        }
    }

    private void ResetJump()
    {
        isReadyToJump = true;
    }

    private void ControlCamera()
    {
        //Camera
        horizontalAxis.Value += cameraInput.x * sensX;
        verticalAxis.Value += -cameraInput.y * sensY;

        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, -75f, 75f);

        OrbitalCamera.HorizontalAxis = horizontalAxis;
        OrbitalCamera.VerticalAxis = verticalAxis;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.normal.textColor = Color.orangeRed;

        float percentage = 0.1f; // 5% of screen height
        style.fontSize = Mathf.RoundToInt(Screen.height * percentage);


        GUI.Label(new Rect(10, 10, Screen.width, style.fontSize), "Move Speed: " + rb.linearVelocity.magnitude, style);
    }
}