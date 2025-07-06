using System;
using System.Collections;
using DefaultNamespace;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject persistentUI;
    public UnityEvent TUIInGame;
    
    //Object Properties
    Rigidbody rb;
    
    //Move
    private PlayerInput playerInput;
    private Vector2 moveInput;
    
    public float maxSpeed = 10f;
    
    //Camera
    private Vector2 cameraInput;
    public CinemachineCamera CineCamera;
    public CinemachineOrbitalFollow OrbitalCamera;
    private InputAxis horizontalAxis;
    private InputAxis verticalAxis;
    
    public float sensX;
    public float sensY;
    
    //Object Atlas
    public Transform orientation;
    
    float xRotation;
    float yRotation;
    
    private void Awake()
    {
        if (UIManager.Instance == null)
        {
            Instantiate(persistentUI);
        }
        UIManager.Instance.TUIInGame.SetGameStartedFlag(true);
    }
    
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }
    
    void Update()
    {
        //Camera
        horizontalAxis.Value += cameraInput.x;
        verticalAxis.Value += cameraInput.y;

        OrbitalCamera.HorizontalAxis = horizontalAxis;
        OrbitalCamera.VerticalAxis = verticalAxis;
        
        //Move
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        cameraInput = playerInput.actions["Camera"].ReadValue<Vector2>();
        
    }

    void FixedUpdate()
    {
        //Move
        Vector2 normalizedMoveInput = moveInput.normalized;
        
        rb.AddForce(new Vector3(normalizedMoveInput.x, 0f, normalizedMoveInput.y) * (maxSpeed * Time.fixedDeltaTime));
        
        
    }
}
