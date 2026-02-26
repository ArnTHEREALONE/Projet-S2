using UnityEngine;

public class StickPlayerToPlatform : MonoBehaviour
{
    public GameObject player;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(transform);
            Debug.Log("Player parented to platform");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(null);
            Debug.Log("Player unparented from platform");
        }
    }
}

