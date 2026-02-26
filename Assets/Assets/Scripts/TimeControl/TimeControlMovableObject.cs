using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TimeControlMovableObject : MonoBehaviour
{
    [Header("Recording")]
    public float recordDuration = 5f;
    public float movementThreshold = 0.05f;

    [Header("Rewind")]
    public float rewindSpeedMultiplier = 1f;
    public float deadZone = 0.1f;
    public float rewindCooldown = 1f;

    [Header("Post Rewind")]
    public float freezeDuration = 0.2f;
    public float resumeDuration = 1f;

    private List<ObjectState> states = new List<ObjectState>();
    private Rigidbody rb;

    private bool isRewinding = false;
    private bool isFreezing = false;
    private bool isResuming = false;

    private float freezeTimer = 0f;
    private float resumeTimer = 0f;
    private float nextAvailableRewindTime = 0f;

    private Vector3 velocityToRestore = Vector3.zero;
    private Vector3 angularVelocityToRestore = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    void Update()
    {
        if (Gamepad.current == null)
            return;

        float triggerValue = Gamepad.current.leftTrigger.ReadValue();

        if (triggerValue > deadZone && Time.time >= nextAvailableRewindTime && !isFreezing && !isResuming)
        {
            isRewinding = true;
        }
        else
        {
            if (isRewinding && triggerValue <= deadZone)
            {
                isRewinding = false;
                StartFreezeAfterRewind();
            }
            else if (triggerValue <= deadZone)
            {
                isRewinding = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            PerformRewind();
            return;
        }

        if (isFreezing)
        {
            HandleFreeze();
            return;
        }

        if (isResuming)
        {
            HandleResume();
            return;
        }
        RecordState();
    }

    void RecordState()
    {
        if (rb.linearVelocity.magnitude < movementThreshold &&
            rb.angularVelocity.magnitude < movementThreshold)
        {
            return;
        }

        rb.isKinematic = false;

        int maxStates = Mathf.RoundToInt(recordDuration / Time.fixedDeltaTime);

        if (states.Count > maxStates)
        {
            states.RemoveAt(states.Count - 1);
        }

        states.Insert(0, new ObjectState(
            transform.position,
            transform.rotation,
            rb.linearVelocity,
            rb.angularVelocity));
    }

    void PerformRewind()
    {
        rb.isKinematic = true;

        int steps = Mathf.Max(1, Mathf.RoundToInt(rewindSpeedMultiplier));

        for (int i = 0; i < steps; i++)
        {
            if (states.Count == 0)
            {
                isRewinding = false;
                StartFreezeAfterRewind();
                return;
            }

            ObjectState s = states[0];

            transform.position = s.position;
            transform.rotation = s.rotation;

            velocityToRestore = s.velocity;
            angularVelocityToRestore = s.angularVelocity;

            states.RemoveAt(0);
        }
    }

    void StartFreezeAfterRewind()
    {
        isFreezing = true;
        freezeTimer = freezeDuration;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void HandleFreeze()
    {
        freezeTimer -= Time.fixedDeltaTime;

        if (freezeTimer <= 0f)
        {
            isFreezing = false;
            isResuming = true;
            resumeTimer = 0f;

            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void HandleResume()
    {
        resumeTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(resumeTimer / Mathf.Max(0.0001f, resumeDuration));
        float multiplier = Mathf.SmoothStep(0f, 1f, t);

        //rb.linearVelocity = velocityToRestore * multiplier;
        //rb.angularVelocity = angularVelocityToRestore * multiplier;

        if (t >= 1f)
        {
            isResuming = false;
            nextAvailableRewindTime = Time.time + rewindCooldown;
        }
    }

    public void ClearHistory()
    {
        states.Clear();
    }

}

public class ObjectState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public ObjectState(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
    {
        position = pos;
        rotation = rot;
        velocity = vel;
        angularVelocity = angVel;
    }

}