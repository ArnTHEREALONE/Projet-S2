using UnityEngine;

public abstract class CharacterControlManager : MonoBehaviour
{
    protected Rigidbody rb;
    protected PlayerController controller;

    protected virtual void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Engage()
    {
        //behavior to the behavior
    }
    public virtual void Disengage()
    {
        //behavior to stop the behavior
    }
}