using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string stateName = "MyAnimation";

    [Header("Speed")]
    public float speed = 0.5f;

    private float animTime = 0f;
    private int state;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        state = Animator.StringToHash(stateName);

        animator.speed = 0f;
        animator.Play(state, 0, animTime);
    }


    void Update()
    {
        float direction = 0f;

        if (Input.GetKey("e"))
            direction += 1f;

        if (Input.GetKey("q"))
            direction -= 1f;

        if (direction != 0f)
        {
            animTime += direction * speed * Time.deltaTime;
            animTime = Mathf.Clamp01(animTime);

            animator.Play(state, 0, animTime);
            animator.Update(0f);
        }
    }

}
