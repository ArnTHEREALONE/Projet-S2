using UnityEngine;

public class MovingPlatformTracker : MonoBehaviour
{
    public Vector3 platformVelocity { get; private set; }

    private Rigidbody platformRb;
    private Vector3 lastPlatformPos;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            platformRb = collision.rigidbody;

            if (platformRb != null)
                lastPlatformPos = platformRb.position;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (platformRb != null)
        {
            platformVelocity = platformRb.linearVelocity;
        }
        else
        {
            Transform t = collision.transform;

            Vector3 currentPos = t.position;
            platformVelocity = (currentPos - lastPlatformPos) / Time.deltaTime;
            lastPlatformPos = currentPos;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == platformRb)
        {
            platformRb = null;
            platformVelocity = Vector3.zero;
        }
    }
}
