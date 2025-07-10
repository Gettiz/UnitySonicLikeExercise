using System;
using System.Collections;
using System.Numerics;
using DefaultNamespace;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject persistentUI;
    public UnityEvent TUIInGame;

    [Header("Player")] private CapsuleCollider playerCollider;
    private Rigidbody rb;

    [Header("Move")] public float moveSpeed = 10f;
    public float maxSpeed = 20;
    public bool activeMaxSpeedCap = true;

    public float groundDamping = 5f;
    public float playerRotationSpeed = 15;
    public float playerNormalRotationSpeed = 500f;
    public float playerGravity = 50;

    public AnimationCurve decelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float decelerationTimer;

    private bool isGrounded;
    private bool hasGroundedStarted = true;
    public float hasGroundedStartedTimer;
    public float lerpOffGround = 5;
    private Vector3 projectedMoveDirection;
    private float projectedAngle;
    public float angleDamping = 5f;
    public float accelerationTimer = 2;

    public float OverSpeedBreak = 5;
    public float momentumFriction = 5;

    private Vector3 LastSavedDirection;

    RaycastHit nHit;
    private Vector3 groundNormal;

    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 aimDirection;
    private Vector3 finalDesiredSpeed;
    private Vector3 projectedLookDirection;

    [Header("Jump")] public float jumpForce = 20f;
    public float jumpCooldown = 0.1f;
    public float airMultiplier = 0.4f;

    private bool isReadyToJump = true;

    [Header("Ground Check")] public float playerHeight = 2;
    public LayerMask groundLayer;

    [Header("Camera")] public Vector2 cameraInput;
    public CinemachineCamera CineCamera;
    public CinemachineOrbitalFollow OrbitalCamera;
    public float CamFollowSpeed = 5f;
    public float CamWaitToFollowSpeed = 5f;
    private float lastInputCamInTime;
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
        lastInputCamInTime = Time.time;
    }

    private void Update()
    {
        GroundCheck();

        PlayerInputMove();

        ControlInputCamera();
    }

    private void FixedUpdate()
    {
        ControlMovement();

        ControlSpeedVelocity();

        ControlRotation();
    }

    private void GroundCheck()
    {
        //Ground Check
        if (Physics.Raycast(transform.position, -transform.up, out nHit, playerHeight * 0.5f + 0.2f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            groundNormal = nHit.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.Lerp(groundNormal, Vector3.up, Time.deltaTime * lerpOffGround);
        }


        //Is on ground?
        if (isGrounded)
        {
            //rb.linearDamping = groundDamping;
            if (hasGroundedStarted)
            {
                StartCoroutine(lerpDamping());
                hasGroundedStarted = false;
            }
        }
        else
        {
            StopCoroutine(lerpDamping());
            hasGroundedStarted = true;
            rb.linearDamping = 0;
        }
    }

    IEnumerator lerpDamping()
    {
        float elapsedTime = 0;
        while (elapsedTime < hasGroundedStartedTimer && isGrounded)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / hasGroundedStartedTimer;

            rb.linearDamping = Mathf.Lerp(rb.linearDamping, groundDamping, t);
            yield return null;
        }
    }

    private void PlayerInputMove()
    {
        //Move
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 camForward = cameraOrientation.forward;
        Vector3 camRight = cameraOrientation.right;

        camForward = Vector3.ProjectOnPlane(camForward, transform.up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, transform.up).normalized;

        //for movement direction of Player
        moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }

    private void ControlInputCamera()
    {
        //Camera
        cameraInput = playerInput.actions["Camera"].ReadValue<Vector2>();

        if (math.abs(cameraInput.x) > 0.01f || math.abs(cameraInput.y) > 0.01f)
        {
            horizontalAxis.Value += cameraInput.x * sensX;
            verticalAxis.Value += -cameraInput.y * sensY;

            verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, -75f, 75f);

            lastInputCamInTime = Time.time;
        }
        else
        {
            if (Time.time >= lastInputCamInTime + CamWaitToFollowSpeed)
            {
                float CurrentAngleX = Mathf.Atan2(projectedLookDirection.x, projectedLookDirection.z) * Mathf.Rad2Deg;
                horizontalAxis.Value = Mathf.LerpAngle(horizontalAxis.Value, CurrentAngleX, Time.deltaTime * CamFollowSpeed);

                //float CurrentAngleY = Mathf.LerpAngle(verticalAxis.Value, projectedMoveDirection.y, Time.deltaTime * CamFollowSpeed*2);
                //verticalAxis.Value = CurrentAngleY;
                
                /*
                 * Añadir rotacion de camara basado en "y" del jugador.
                 */
            }
        }

        OrbitalCamera.HorizontalAxis = horizontalAxis;
        OrbitalCamera.VerticalAxis = verticalAxis;
    }

    private void ControlMovement()
    {
        //move straight based on normals
        projectedMoveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
        //get player angle
        projectedAngle = Vector3.Dot(projectedMoveDirection, Vector3.up);

        if (isGrounded)
        {
            //accelerate // decelerate
            if (moveDirection.magnitude > 0.1f)
            {
                decelerationTimer += Time.deltaTime;
                LastSavedDirection = projectedMoveDirection;
                moveSpeed = Mathf.Lerp(moveSpeed, maxSpeed, accelerationTimer * Time.deltaTime);
            }
            else
            {
                decelerationTimer -= Time.deltaTime;
                decelerationTimer = Mathf.Clamp(decelerationTimer, 0f, accelerationTimer / 2);
                float curveTime = decelerationTimer / accelerationTimer;
                float curve = decelerationCurve.Evaluate(curveTime);

                moveSpeed = Mathf.Lerp(0, moveSpeed, curve);
            }

            // Vector Direction
            Vector3 desiredMoveDirection = moveDirection.magnitude > 0.1f ? projectedMoveDirection : LastSavedDirection;

            // Set final Speed
            finalDesiredSpeed = desiredMoveDirection * ((moveSpeed - projectedAngle * angleDamping) * 10);
            rb.AddForce(finalDesiredSpeed, ForceMode.Force);
        }

        if (!isGrounded)
        {
            rb.AddForce(moveDirection * (moveSpeed * 10 * airMultiplier), ForceMode.Force);
            Gravity();
        }
    }

    private void ControlSpeedVelocity()
    {
        if (activeMaxSpeedCap)
        {
            Vector3 currentMomentum = rb.linearVelocity;
            if (currentMomentum.magnitude > 5f)
            {
                float frictionMagnitude;
                if (currentMomentum.magnitude > maxSpeed * 2)
                {
                    frictionMagnitude = OverSpeedBreak;
                }
                else
                {
                    frictionMagnitude = -momentumFriction;
                }

                Vector3 frictionForce = -currentMomentum.normalized * frictionMagnitude;
                rb.AddForce(frictionForce, ForceMode.Force);
            }
        }
    }

    private void ControlRotation()
    {
        //Align Player towards normals
        float angleToGroundNormal = Vector3.Angle(transform.up, groundNormal);
        if (angleToGroundNormal > 0.1f)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;

            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * playerNormalRotationSpeed));
        }

        //Align Player towards move direction
        if (moveDirection.magnitude > 0.1f)
        {
            projectedLookDirection = Vector3.ProjectOnPlane(moveDirection, transform.up).normalized;
            if (projectedLookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(projectedLookDirection, transform.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * playerRotationSpeed));
            }
        }
    }

    /*/////////////////////////////////// Callable movement ///////////////////////////////////*/

    private void Gravity()
    {
        rb.linearVelocity -= Vector3.up * playerGravity * Time.deltaTime;
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            //rb.linearVelocity = Vector3.Project(rb.linearVelocity, rb.transform.up);

            //TODO Take into account for BunnyJump Mechanic
            Invoke("ResetJump", jumpCooldown);
        }
    }

    private void ResetJump()
    {
        isReadyToJump = true;
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