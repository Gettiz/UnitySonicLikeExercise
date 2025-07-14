using UnityEngine;

namespace PlayerMovement
{
    public class ControlRotationClass : MonoBehaviour
    {
        private PlayerController player;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }
        public void ControlRotation()
        {
            //Align Player towards normals
            float angleToGroundNormal = Vector3.Angle(player.transform.up, player.groundNormal);
            if (angleToGroundNormal > 0.1f)
            {
                Quaternion targetRotation = Quaternion.FromToRotation(player.transform.up, player.groundNormal) * player.transform.rotation;

                player.rb.MoveRotation(Quaternion.Slerp(player.transform.rotation, targetRotation, Time.fixedDeltaTime * player.playerNormalRotationSpeed));
            }

            //Align Player towards move direction
            if (player.moveDirection.magnitude > 0.1f)
            {
                player.projectedLookDirection = Vector3.ProjectOnPlane(player.moveDirection, player.transform.up).normalized;
                if (player.projectedLookDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(player.projectedLookDirection, player.transform.up);
                    player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, lookRotation, Time.fixedDeltaTime * player.playerRotationSpeed));
                }
            }
        }
    }
}