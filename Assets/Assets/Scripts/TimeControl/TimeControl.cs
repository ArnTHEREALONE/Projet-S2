using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Collections;

public class TimeControl : MonoBehaviour
{
    public List<Animator> animators = new List<Animator>();

    [Header("Settings")]
    public float timeSpeed = 1f;
    public float returnToNormalDuration = 2f;
    private float deadZone = 0.1f;

    private Dictionary<Animator, float> animTimes = new Dictionary<Animator, float>();
    private Dictionary<Animator, int> stateHashes = new Dictionary<Animator, int>();

    public AudioSource Rewind;
    public AudioClip rewind1;

    private bool isManualControl = false;
    private bool isReturning = false;
    private float returnTimer = 0f;

    void Start()
    {
        foreach (var animator in animators)
        {
            if (animator == null) continue;

            var info = animator.GetCurrentAnimatorStateInfo(0);
            animTimes[animator] = info.normalizedTime % 1f;
            stateHashes[animator] = info.shortNameHash;

            animator.speed = 1f;
        }
    }

    void Update()
    {
        if (Gamepad.current == null)
            return;

        float right = Gamepad.current.rightTrigger.ReadValue();
        float left = Gamepad.current.leftTrigger.ReadValue();

        float direction = 0f;

        if (right > deadZone)
        {
            direction += right;
        }
        if (left > deadZone) direction -= left;

        if (direction != 0f)
        {
            if (!isManualControl)
            {
                EnterManualMode();
            }

            ManualUpdate(direction);
        }
        else
        {
            if (isManualControl)
            {
                ExitManualMode();
            }

            if (isReturning)
            {
                SmoothReturnToNormal();
            }
        }
    }

    void EnterManualMode()
    {
        isManualControl = true;
        isReturning = false;

        foreach (var animator in animators)
        {
            if (animator == null) continue;

            var info = animator.GetCurrentAnimatorStateInfo(0);
            var clips = animator.GetCurrentAnimatorClipInfo(0);

            float normalized = info.normalizedTime;

            if (clips != null && clips.Length > 0 && clips[0].clip.isLooping)
            {
                animTimes[animator] = normalized % 1f;
            }
            else
            {
                animTimes[animator] = Mathf.Clamp01(normalized);
            }

            animator.speed = 0f;
        }
    }

    void ManualUpdate(float direction)
    {
        foreach (var animator in animators)
        {
            if (animator == null) continue;

            int state = stateHashes[animator];

            animTimes[animator] += direction * timeSpeed * Time.deltaTime;

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.isLooping)
            {
                animTimes[animator] = Mathf.Repeat(animTimes[animator], 1f);
            }
            else
            {
                animTimes[animator] = Mathf.Clamp01(animTimes[animator]);
            }

            animator.Play(state, 0, animTimes[animator]);
            animator.Update(0f);
        }
    }

    void ExitManualMode()
    {
        isManualControl = false;
        isReturning = true;
        returnTimer = 0f;

        foreach (var animator in animators)
        {
            if (animator == null) continue;
            animator.speed = 0f;
        }
    }

    void SmoothReturnToNormal()
    {
        returnTimer += Time.deltaTime;

        float t = returnTimer / returnToNormalDuration;
        float speed = Mathf.SmoothStep(0f, 1f, t);

        foreach (var animator in animators)
        {
            if (animator == null) continue;
            animator.speed = speed;
        }

        if (t >= 1f)
        {
            isReturning = false;
        }
    }
}
