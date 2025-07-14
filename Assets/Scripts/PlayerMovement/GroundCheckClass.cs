using System.Collections;
using UnityEngine;

public class GroundCheckClass : MonoBehaviour
{
    private PlayerController player;
    public RaycastHit nHit;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    public void GroundCheck()
    {
        //Ground Check
        if (Physics.Raycast(player.transform.position, -player.transform.up, out nHit, player.playerHeight * 0.5f + 0.2f, player.groundLayer, QueryTriggerInteraction.Ignore))
        {
            if (!player.canGroundbeDeactivated)
            {
                player.isGrounded = true;
            }
            
            player.groundNormal = nHit.normal;
        }
        else
        {
            if (!player.canGroundbeDeactivated)
            {
                player.isGrounded = false;
            }

            player.groundNormal = Vector3.Lerp(player.groundNormal, Vector3.up, Time.deltaTime * player.lerpOffGround);
        }

        //Is on ground?
        if (player.isGrounded)
        {
            //rb.linearDamping = groundDamping;
            if (player.hasGroundedStarted)
            {
                StartCoroutine(lerpDamping());
                player.hasGroundedStarted = false;
            }
        }
        else
        {
            StopCoroutine(lerpDamping());
            player.hasGroundedStarted = true;
        }

        player.groundNormalAngle = Vector3.Angle(player.groundNormal, Vector3.up);
        if (player.groundNormalAngle > 45)
        {
            player.rb.linearVelocity -= Vector3.up * (player.playerGravity / 2) * Time.deltaTime;
        }
    }
    IEnumerator lerpDamping()
    {
        float elapsedTime = 0;
        while (elapsedTime < player.hasGroundedStartedTimer && player.isGrounded)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / player.hasGroundedStartedTimer;

            player.rb.linearDamping = Mathf.Lerp(player.rb.linearDamping, player.groundDamping, t);
            yield return null;
        }
    }
}