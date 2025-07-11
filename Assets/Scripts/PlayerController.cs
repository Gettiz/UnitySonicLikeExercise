using System;
using System.Collections;
using System.Numerics;
using DefaultNamespace;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEditor;
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

    public Vector3 vectorCameraToLookAt;
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

    public Vector3 inputToCameraPosition;
    public Quaternion inputToCameraRotation;

    public float cameraLerpLagFollowRotation = 5f;
    public float cameraLerpLagFollowRotationMultiplier = 5f;
    public float cameraNormalToRotation = 0.1f;
    private Vector3 velocityFollowPosition;
    public float cameraLerpLagFollowPosition = 0.15f;

    public float cameraPositionDistance = 6f;
    public float cameraPositionHeight= 0.5f;

    public GameObject cameraMain;
    public GameObject cameraTargetPosition;

    private float xRotationValue;
    private float yRotationValue;
    private float zRotationValue;
    
    public float sensX;
    public float sensY;
    

    //[Header("Object Atlas")] 

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

        Vector3 camForward = cameraMain.transform.forward;
        Vector3 camRight = cameraMain.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, transform.up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, transform.up).normalized;

        //for movement direction of Player
        moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;
    }

    private void ControlInputCamera()
    {
        //Camera
        cameraInput = playerInput.actions["Camera"].ReadValue<Vector2>();

        //Rotate Camera based on input 
        if (math.abs(cameraInput.x) > 0.01f && rb.linearVelocity.magnitude < 25 || math.abs(cameraInput.y) > 0.01f && rb.linearVelocity.magnitude < 25)
        {
            xRotationValue += cameraInput.x * sensX;
            yRotationValue += -cameraInput.y * sensY;
            yRotationValue = Mathf.Clamp(yRotationValue, -75f, 75f);
            
            inputToCameraRotation = Quaternion.Euler(yRotationValue, xRotationValue, zRotationValue);
        }
        //Slowly Rotate Camera to player position if input is pressed
        if (math.abs(moveDirection.magnitude) > 0.01f && rb.linearVelocity.magnitude < 25)
        {
            Quaternion lookrotationcamera = Quaternion.LookRotation(moveDirection);
            xRotationValue = cameraMain.transform.eulerAngles.y;
            yRotationValue = cameraMain.transform.eulerAngles.x;
            
            inputToCameraRotation = Quaternion.Lerp(inputToCameraRotation, lookrotationcamera, cameraLerpLagFollowRotation * Time.deltaTime);
            
        }
        
        if (rb.linearVelocity.magnitude >= 25)
        {
            Quaternion lookrotationcamera = Quaternion.LookRotation(moveDirection);
            xRotationValue = cameraMain.transform.eulerAngles.y;
            yRotationValue = cameraMain.transform.eulerAngles.x;
            
            inputToCameraRotation = Quaternion.Lerp(inputToCameraRotation, lookrotationcamera, cameraLerpLagFollowRotation * Time.deltaTime * cameraLerpLagFollowRotationMultiplier);
            
            Vector3 lookAtNormal = Vector3.Lerp(cameraMain.transform.up,groundNormal,cameraNormalToRotation);
            
            cameraMain.transform.LookAt(cameraTargetPosition.transform, lookAtNormal);
        }
        else
        {
            cameraMain.transform.LookAt(cameraTargetPosition.transform);
        }
        
        ////How Far my camera will be and "Lag"////
        Vector3 desiredPosition = cameraTargetPosition.transform.position - (cameraTargetPosition.transform.forward * cameraPositionDistance) + (Vector3.up * cameraPositionHeight);
        cameraMain.transform.position = Vector3.SmoothDamp(cameraMain.transform.position, desiredPosition, ref velocityFollowPosition,cameraLerpLagFollowPosition);
        
        ////Rotation
        cameraTargetPosition.transform.rotation = inputToCameraRotation;
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