using UnityEngine;

namespace PlayerMovement
{
    public class ControlSpeedVelocityClass : MonoBehaviour
    {
        private PlayerController player;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }
        public void ControlSpeedVelocity()
        {
            if (player.activeMaxSpeedCap)
            {
                Vector3 currentMomentum = player.rb.linearVelocity;
                if (currentMomentum.magnitude > 5f)
                {
                    float frictionMagnitude;
                    if (currentMomentum.magnitude > player.maxSpeed * 2)
                    {
                        frictionMagnitude = player.OverSpeedBreak;
                    }
                    else
                    {
                        frictionMagnitude = -player.momentumFriction;
                    }

                    Vector3 frictionForce = -currentMomentum.normalized * frictionMagnitude;
                    player.rb.AddForce(frictionForce, ForceMode.Force);
                }
            }
        }
    }
}