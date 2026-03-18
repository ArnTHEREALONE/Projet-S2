using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    public SplineContainer splineContainer;

    [Header("Movement")]
    public float speed = 5f;
    public bool loop = true;
    public bool alignRotation = true;

    private float distanceTravelled = 0f;
    private float splineLength;

    void Start()
    {
        if (splineContainer != null)
        {
            splineLength = splineContainer.Spline.GetLength();
        }
    }

    void Update()
    {
        if (splineContainer == null) return;

        distanceTravelled += speed * Time.deltaTime;

        float t = distanceTravelled / splineLength;

        if (loop)
            t = Mathf.Repeat(t, 1f);
        else
            t = Mathf.Clamp01(t);

        Vector3 position = splineContainer.EvaluatePosition(t);
        transform.position = position;

        if (alignRotation)
        {
            Vector3 forward = splineContainer.EvaluateTangent(t);
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}
