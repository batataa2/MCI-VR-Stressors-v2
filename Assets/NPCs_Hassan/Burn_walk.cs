using UnityEngine;
using UnityEngine.AI;

public class NavMeshWander : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderTimer = 3f;

    public Animator animator;  // Reference to your Animator component

    private NavMeshAgent agent;
    private float timer;

    private float horizontal; // for smoothing
    private float smoothTime = 0.1f; // smoothing duration
    private float horizontalVelocity; // internal velocity for smoothing

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(
                transform.position,
                out NavMeshHit hit,
                2f,
                NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }

        timer = wanderTimer;
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir += transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                timer = 0f;
            }
        }

        // Calculate turning amount based on agent steering
        Vector3 desiredDirection = agent.steeringTarget - transform.position;
        desiredDirection.y = 0f;

        if (desiredDirection.magnitude > 0.1f)
        {
            // Get signed angle between current forward and desired direction
            float angle = Vector3.SignedAngle(transform.forward, desiredDirection.normalized, Vector3.up);

            // Normalize angle to range [-1,1] (assuming max turn angle 90 degrees)
            float targetHorizontal = Mathf.Clamp(angle / 90f, -1f, 1f);

            // Smoothly interpolate horizontal parameter
            horizontal = Mathf.SmoothDamp(horizontal, targetHorizontal, ref horizontalVelocity, smoothTime);
        }
        else
        {
            // No turning needed
            horizontal = Mathf.SmoothDamp(horizontal, 0f, ref horizontalVelocity, smoothTime);
        }

        // Update animator parameter
        if (animator != null)
        {
            animator.SetFloat("Horizontal", horizontal);
        }
    }
}
