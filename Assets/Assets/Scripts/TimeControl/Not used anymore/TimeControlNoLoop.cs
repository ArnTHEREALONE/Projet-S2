using UnityEngine;
using UnityEngine.InputSystem;

public class TimeControlNoLoop : MonoBehaviour
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
        animTime = CaptureNormalizedTimeSafely(info);
    }

    void Update()
    {
        float direction = 0f;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightTrigger.ReadValue() > 0.1f) direction += timeSpeed;
            if (Gamepad.current.leftTrigger.ReadValue() > 0.1f) direction -= timeSpeed;
        }
        else if (Gamepad.current == null)
        {
            Debug.Log("No manette");
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
                animTime = CaptureNormalizedTimeSafely(info);
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
                animator.Update(0f);
            }
        }
    }

    private float CaptureNormalizedTimeSafely(AnimatorStateInfo info)
    {
        float currentNormalized = info.normalizedTime;

        if (info.shortNameHash != state)
            return 1f;

        var clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips != null && clips.Length > 0)
        {
            var clip = clips[0].clip;
            if (clip != null && clip.isLooping)
            {
                return currentNormalized % 1f;
            }

            return Mathf.Clamp01(currentNormalized);
        }

        //pas nécessaire
        return Mathf.Clamp01(currentNormalized);
    }
}
