using UnityEngine;

public class SplineTriggerActivator : MonoBehaviour
{
    public SplineFollowerControlled splineFollower; // Référence au script à activer
    public bool activateOnEnter = true; // True = start movement, False = stop movement

    private void OnTriggerEnter(Collider other)
    {
        if (splineFollower == null) return;

        // Ici on peut filtrer par tag si nécessaire, par exemple "Player"
        if (other.CompareTag("Player"))
        {
            splineFollower.SetMovementActive(activateOnEnter);
        }
    }
}