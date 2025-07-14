using System;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerMovement
{
    public class ControlInputCameraClass : MonoBehaviour
    {
        private PlayerController player;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        public void ControlInputCamera()
    {
        //Camera
        player.cameraInput = player.playerInput.actions["Camera"].ReadValue<Vector2>();

        //Rotate Camera based on input 
        if (math.abs(player.cameraInput.x) > 0.01f && player.rb.linearVelocity.magnitude < 20 || math.abs(player.cameraInput.y) > 0.01f && player.rb.linearVelocity.magnitude < 20)
        {
            player.xRotationValue += player.cameraInput.x * player.sensX;
            player.yRotationValue += -player.cameraInput.y * player.sensY;
            player.yRotationValue = Mathf.Clamp(player.yRotationValue, -75f, 75f);

            player.inputToCameraRotation = Quaternion.Euler(player.yRotationValue, player.xRotationValue, player.zRotationValue);
        }

        //Slowly Rotate Camera to player position if input is pressed
        if (math.abs(player.moveDirection.magnitude) > 0.01f && player.rb.linearVelocity.magnitude < 20)
        {
            Quaternion lookrotationcamera = Quaternion.LookRotation(player.LastSavedDirection);

            player.inputToCameraRotation = Quaternion.Lerp(player.inputToCameraRotation, lookrotationcamera, player.cameraLerpLagFollowRotation * Time.deltaTime);
        }

        if (player.rb.linearVelocity.magnitude >= 20)
        {
            Quaternion lookrotationcamera = Quaternion.LookRotation(player.LastSavedDirection);
            player.xRotationValue = player.cameraMain.transform.eulerAngles.y;
            player.yRotationValue = player.cameraMain.transform.eulerAngles.x;

            player.inputToCameraRotation = Quaternion.Lerp(player.inputToCameraRotation, lookrotationcamera,
                player.cameraLerpLagFollowRotation * Time.deltaTime * player.cameraLerpLagFollowRotationMultiplier);

            Vector3 lookAtNormal = Vector3.Lerp(player.cameraMain.transform.up, player.groundNormal, player.cameraNormalToRotation);

            player.cameraMain.transform.LookAt(player.cameraTargetPosition.transform, lookAtNormal);
        }
        else
        {
            player.cameraMain.transform.LookAt(player.cameraTargetPosition.transform);
        }

        ////How Far my camera will be and "Lag"////
        Vector3 desiredPosition = player.cameraTargetPosition.transform.position - (player.cameraTargetPosition.transform.forward * player.cameraPositionDistance) +
                                  (Vector3.up * player.cameraPositionHeight);
        player.cameraMain.transform.position =
            Vector3.SmoothDamp(player.cameraMain.transform.position, desiredPosition, ref player.velocityFollowPosition, player.cameraLerpLagFollowPosition);

        ////Rotation
        player.cameraTargetPosition.transform.rotation = player.inputToCameraRotation;
    }
    }
}