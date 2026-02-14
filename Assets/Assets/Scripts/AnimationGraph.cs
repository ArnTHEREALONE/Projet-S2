using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class AnimationGraph : MonoBehaviour
{
    public AnimationClip clip;
    public float speed = 1f;

    private PlayableGraph graph;
    private AnimationClipPlayable clipPlayable;
    private float currentTime;

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        graph = PlayableGraph.Create("AnimationGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        clipPlayable = AnimationClipPlayable.Create(graph, clip);
        clipPlayable.SetSpeed(0);

        var output = AnimationPlayableOutput.Create(
            graph,
            "Animation",
            animator
        );

        output.SetSourcePlayable(clipPlayable);
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
            currentTime += direction * speed * Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0f, clip.length);

            clipPlayable.SetTime(currentTime);
            graph.Evaluate(0f);
        }
    }

    void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }
}
