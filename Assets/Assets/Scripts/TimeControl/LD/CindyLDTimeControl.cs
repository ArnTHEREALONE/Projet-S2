using UnityEngine;

public class TriggerTimeModifier : MonoBehaviour
{
    public Animator animator;
    public TimeControl timeControl;

    public float timeSpeed;
    public float returnToNormalDuration;

    private void Start()
    {
        animator.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.enabled = true;

            timeControl.timeSpeed = timeSpeed;
            timeControl.returnToNormalDuration = returnToNormalDuration;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.enabled = false;
        }
    }
}