using UnityEngine;

// Generate by gpt
public class SmokeTrailController : MonoBehaviour
{
    public float trailTime = 2f; // Duration of the smoke trail
    public float updateInterval = 0.1f; // Interval between trail updates
    public Transform trailTransform; // Object to follow and create the trail

    private LineRenderer lineRenderer;
    private float elapsedTime = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, trailTransform.position);

        InvokeRepeating(nameof(UpdateTrail), updateInterval, updateInterval);
    }

    private void UpdateTrail()
    {
        elapsedTime += updateInterval;

        // Create a new position for the trail
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, trailTransform.position);

        // Remove the oldest position from the trail if it has exceeded the trail time
        if (elapsedTime >= trailTime)
        {
            lineRenderer.positionCount--;
            elapsedTime -= updateInterval;
        }
    }
}