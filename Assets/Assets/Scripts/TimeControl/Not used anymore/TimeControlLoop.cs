using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationControl : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string stateName = "MyAnimation";

    [Header("Speed")]
    public float speed = 0.5f, timeSpeed = 1.5f;

    private float animTime = 0f;
    private int state;
    private bool isManualControl = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        state = Animator.StringToHash(stateName);

        animator.Play(state);
        animator.Update(0f);

        var info = animator.GetCurrentAnimatorStateInfo(0);
        animTime = info.length > 0 ? info.normalizedTime % 1f : 0f;
    }

    void Update()
    {
        float direction = 0f;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightTrigger.ReadValue() > 0.1f) direction += timeSpeed;
            if (Gamepad.current.leftTrigger.ReadValue() > 0.1f) direction -= timeSpeed;
        }

        if (Input.GetButton("FF")) direction += timeSpeed;
        if (Input.GetButton("Rewind")) direction -= timeSpeed;

        if (direction != 0f)
        {
            if (!isManualControl)
            {
                isManualControl = true;
                animator.speed = 0f;

                var info = animator.GetCurrentAnimatorStateInfo(0);
                animTime = (info.length > 0) ? (info.normalizedTime % 1f) : 0f;
            }

            animTime += direction * speed * Time.deltaTime;
            animTime = Mathf.Clamp01(animTime);

            animator.Play(state, 0, animTime);
            animator.Update(0f);
        }
        else
        {
            if (isManualControl)
            {
                isManualControl = false;
                animator.speed = 1f;
                animator.Play(state, 0, animTime);
            }
        }
    }
}
