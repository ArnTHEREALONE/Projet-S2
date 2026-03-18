using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SplineTimeControl : MonoBehaviour
{
    public List<SplineFollowerControlled> followers = new List<SplineFollowerControlled>();

    [Header("Settings")]
    public float timeSpeed = 1f;
    public float returnToNormalDuration = 2f;
    private float deadZone = 0.1f;

    private Dictionary<SplineFollowerControlled, float> positions = new Dictionary<SplineFollowerControlled, float>();

    public AudioSource Rewind;
    public AudioClip rewind1;

    private bool isManualControl = false;
    private bool isReturning = false;
    private float returnTimer = 0f;

    void Start()
    {
        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] = f.GetNormalizedPosition();
            f.SetSpeedMultiplier(1f);
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

        if (left > deadZone)
            direction -= left;

        if (direction != 0f)
        {
            if (!isManualControl)
                EnterManualMode();

            ManualUpdate(direction);
        }
        else
        {
            if (isManualControl)
                ExitManualMode();

            if (isReturning)
                SmoothReturnToNormal();
        }
    }

    void EnterManualMode()
    {
        isManualControl = true;
        isReturning = false;

        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] = f.GetNormalizedPosition();
            f.SetSpeedMultiplier(0f);
        }
    }

    void ManualUpdate(float direction)
    {
        foreach (var f in followers)
        {
            if (f == null) continue;

            positions[f] += direction * timeSpeed * Time.deltaTime;

            if (f.loop)
                positions[f] = Mathf.Repeat(positions[f], 1f);
            else
                positions[f] = Mathf.Clamp01(positions[f]);

            f.SetNormalizedPosition(positions[f]);
        }
    }

    void ExitManualMode()
    {
        isManualControl = false;
        isReturning = true;
        returnTimer = 0f;

        foreach (var f in followers)
        {
            if (f == null) continue;
            f.SetSpeedMultiplier(0f);
        }
    }

    void SmoothReturnToNormal()
    {
        returnTimer += Time.deltaTime;

        float t = returnTimer / returnToNormalDuration;
        float speed = Mathf.SmoothStep(0f, 1f, t);

        foreach (var f in followers)
        {
            if (f == null) continue;
            f.SetSpeedMultiplier(speed);
        }

        if (t >= 1f)
            isReturning = false;
    }
}