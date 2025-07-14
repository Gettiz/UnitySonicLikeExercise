using System.Collections;
using PlayerMovement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public GameObject persistentUI;
    public UnityEvent TUIInGame;

    [Header("Player")] 
    public CapsuleCollider playerCollider;
    public Rigidbody rb;
    
    [Header("Ground Check")] 
    public float playerHeight = 2f;
    public bool canGroundbeDeactivated = false;
    public bool isGrounded;
    public Vector3 groundNormal;
    public LayerMask groundLayer;
    public float lerpOffGround = 5f;
    public bool hasGroundedStarted = true;
    public float hasGroundedStartedTimer = 1f;
    public float groundNormalAngle;
    public float playerGravity = 50f;
    public float groundDamping = 5f;
    
    [Header("Move Input")]
    public Vector2 moveInput;
    public PlayerInput playerInput;
    public GameObject cameraMain;
    public Vector3 moveDirection;
    
    [Header("Camera Input")]
    public float xRotationValue;
    public float yRotationValue;
    public float zRotationValue;
    public float sensX = 0.25f; 
    public float sensY = 0.25f;
    public Vector2 cameraInput;
    public Quaternion inputToCameraRotation;
    public float cameraLerpLagFollowRotation = 1f;
    public float cameraLerpLagFollowRotationMultiplier = 5f;
    public float cameraNormalToRotation = 0.1f;
    public GameObject cameraTargetPosition;
    public float cameraPositionDistance = 6f;
    public float cameraPositionHeight = 0.5f;
    public Vector3 velocityFollowPosition;
    public float cameraLerpLagFollowPosition = 0.1f;
    
    [Header("Control Move")]
    public float moveSpeed = 10f;
    public Vector3 projectedMoveDirection;
    public float projectedAngle;
    public bool wasMovingLastFrame;
    public Vector3 LastSavedDirection;
    public float maxSpeed = 20;
    public float maxSpeedTimer = 0.5f;
    public float moveSpeedToStop;
    public float moveTimer;
    public float deaccelerationDuration = 1f;
    public AnimationCurve decelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float angleDamping = 5f;
    public Vector3 finalDesiredSpeed;
    public float airMultiplier = 30f;
    public bool isBouncing = false;
    public float offsetBounce = 1.2f;
    public float playerGravityBounce = 100f;
    public bool isReadyToJump = true;
    public bool hasTurboStarted = false;
    public float turboMultiplier = 100f;
    
    [Header("Control Speed")]
    public bool activeMaxSpeedCap = true;
    public float OverSpeedBreak = 5f;
    public float momentumFriction = 5f;
    
    [Header("Control Rotation")]
    public float playerNormalRotationSpeed = 500f;
    public Vector3 projectedLookDirection;
    public float playerRotationSpeed = 15f;

    [Header("Jump")] 
    public float jumpForce = 20f;
    
    ///Classes
    public GroundCheckClass groundCheckC;

    public PlayerInputClass playerInputC;
    
    public ControlInputCameraClass controlInputCameraC;

    public ControlMovementClass ControlMovementC;

    public ControlSpeedVelocityClass ControlSpeedVelocityC;

    public ControlRotationClass controlRotationC;

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
        groundCheckC.GroundCheck();
            
        playerInputC.PlayerInputMove();

        controlInputCameraC.ControlInputCamera();
    }

    private void FixedUpdate()
    {
        ControlMovementC.ControlMovement();

        ControlSpeedVelocityC.ControlSpeedVelocity();

        controlRotationC.ControlRotation();
    }

    /*/////////////////////////////////// Callable movement ///////////////////////////////////*/

   public void Gravity()
    {
        rb.linearVelocity -= Vector3.up * playerGravity * Time.deltaTime;
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            
        }
    }

    public void PlayerTurbo(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            hasTurboStarted = true;
        }
        else if (context.canceled)
        {
            hasTurboStarted = false;
        }
    }

    public void PlayerBounce(InputAction.CallbackContext context)
    {
        if (context.performed && !isGrounded && !isBouncing)
        {
            isBouncing = true;
            //deactivated conditions for ground
            canGroundbeDeactivated = true;
            //disable gravity
            isGrounded = true;
            //add down force
            rb.AddForce(Vector3.down * playerGravityBounce, ForceMode.Impulse);
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.normal.textColor = Color.orangeRed;

        float percentage = 0.1f;
        style.fontSize = Mathf.RoundToInt(Screen.height * percentage);


        GUI.Label(new Rect(10, 10, Screen.width, style.fontSize), "Move Speed: " + rb.linearVelocity.magnitude, style);
    }
}