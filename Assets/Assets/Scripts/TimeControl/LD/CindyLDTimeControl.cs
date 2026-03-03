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
        animator.enabled = true;

        timeControl.timeSpeed = timeSpeed;
        timeControl.returnToNormalDuration = returnToNormalDuration;
    }

    void OnTriggerExit(Collider other)
    {
        animator.enabled = false;
    }
}