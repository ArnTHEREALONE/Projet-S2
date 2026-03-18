using UnityEngine;

public class StickPlayerToPlatform : MonoBehaviour
{
    public GameObject player;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(transform);
            Debug.Log("gameobj on");
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            player.transform.SetParent(transform);
            Debug.Log("tag on");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(null);
            Debug.Log("gameobj out");
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            player.transform.SetParent(null);
            Debug.Log("tag out");
        }
    }
}

