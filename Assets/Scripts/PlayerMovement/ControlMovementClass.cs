using UnityEngine;

namespace PlayerMovement
{
    public class ControlMovementClass : MonoBehaviour
    {
        private PlayerController player;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }
        public void ControlMovement()
        {
            //move straight based on normals
            player.projectedMoveDirection = Vector3.ProjectOnPlane(player.moveDirection, player.groundNormal).normalized;
            //get player angle
            player.projectedAngle = Vector3.Dot(player.projectedMoveDirection, Vector3.up);

            if (player.isGrounded)
            {
                //accelerate // decelerate
                if (player.moveDirection.magnitude > 0.1f)
                {
                    player.wasMovingLastFrame = true;

                    player.LastSavedDirection = player.projectedMoveDirection;

                    player.moveSpeed = Mathf.Lerp(player.moveSpeed, player.maxSpeed, player.maxSpeedTimer * Time.deltaTime);
                }
                else
                {
                    if (player.wasMovingLastFrame)
                    {
                        player.wasMovingLastFrame = false;
                        player.moveSpeedToStop = player.moveSpeed;
                        player.moveTimer = 0f;
                    }

                    if (player.moveSpeedToStop > 0)
                    {
                        player.moveTimer += Time.deltaTime;
                        float decelerationProgress = player.moveTimer / player.deaccelerationDuration;
                        float curveValue = player.decelerationCurve.Evaluate(decelerationProgress);

                        player.moveSpeed = Mathf.Lerp(player.moveSpeedToStop, 0f, curveValue);
                    }
                }

                // Vector Direction
                Vector3 desiredMoveDirection = player.moveDirection.magnitude > 0.1f ? player.projectedMoveDirection : player.LastSavedDirection;

                // Set final Speed
                player.finalDesiredSpeed = desiredMoveDirection * ((player.moveSpeed - player.projectedAngle * player.angleDamping) * 10);
                player.rb.AddForce(player.finalDesiredSpeed, ForceMode.Force);
            }

            // Can run along walls?
            if (player.rb.linearVelocity.magnitude < 25 && player.groundNormalAngle > 55)
            {
                player.transform.up = Vector3.up;
                player.groundNormal = Vector3.up;
            }

            if (!player.isGrounded)
            {
                player.rb.AddForce(player.moveDirection * player.airMultiplier, ForceMode.Force);
                player.rb.linearDamping = 0.5f;
                player.Gravity();
            }

            // Can Jump again?
            if (!player.isBouncing && player.isGrounded)
            {
                player.isReadyToJump = true;
            }

            // Turbo has been activated?
            if (player.hasTurboStarted)
            {
                player.rb.AddForce(player.LastSavedDirection * player.turboMultiplier, ForceMode.Force);
            }

            // IsBouncing?
            if (player.isBouncing)
            {
                if (Physics.Raycast(player.transform.position, -player.transform.up, player.playerHeight * 0.5f * player.offsetBounce, player.groundLayer, QueryTriggerInteraction.Ignore))
                {
                    player.rb.AddForce(player.groundNormal.normalized * player.rb.linearVelocity.magnitude, ForceMode.Impulse);
                    Invoke("LaunchBounce", 0.2f);
                }
            }
        }

        private void LaunchBounce()
        {
            player.isBouncing = false;
            player.canGroundbeDeactivated = false;
            player.isGrounded = false;
        }
    }
}