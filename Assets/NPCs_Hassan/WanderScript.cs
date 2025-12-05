using UnityEngine;

public class SimpleWander : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float turnSpeed = 140f;
    public float wanderTime = 3f;
    
    private float timer = 0f;
    private Vector3 direction;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        // Move NPC
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Smooth turn
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Timer for new direction
        timer += Time.deltaTime;
        if (timer >= wanderTime)
        {
            PickNewDirection();
        }
    }

    void PickNewDirection()
    {
        timer = 0f;
        direction = Random.insideUnitSphere;
        direction.y = 0;  // Keep flat on ground
    }
}
