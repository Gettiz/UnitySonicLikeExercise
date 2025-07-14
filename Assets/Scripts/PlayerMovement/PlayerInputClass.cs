using System.Collections;
using UnityEngine;

public class PlayerInputClass : MonoBehaviour
{
    private PlayerController player;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    public void PlayerInputMove()
    {
        //Move
        player.moveInput = player.playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 camForward = player.cameraMain.transform.forward;
        Vector3 camRight = player.cameraMain.transform.right;

        camForward = Vector3.ProjectOnPlane(camForward, player.transform.up).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, player.transform.up).normalized;

        //for movement direction of Player
        player.moveDirection = (camForward * player.moveInput.y + camRight * player.moveInput.x).normalized;
    }
}