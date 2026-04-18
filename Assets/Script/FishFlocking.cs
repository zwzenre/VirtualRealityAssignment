using System.Collections.Generic;
using UnityEngine;

public class FishFlocking : MonoBehaviour
{
    public static List<FishFlocking> allFish = new List<FishFlocking>();

    void OnEnable()
    {
        if (!allFish.Contains(this))
            allFish.Add(this);
    }

    void OnDisable()
    {
        allFish.Remove(this);
    }

    [Header("Movement")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3f;
    public float turnSpeed = 1.5f;
    public float acceleration = 2f;

    [Header("Flocking")]
    public float neighborRadius = 3f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.35f;
    public float separationWeight = 1.8f;

    [Header("Obstacle")]
    public LayerMask obstacleLayer;
    public float obstacleCheckDistance = 2f;
    public float obstacleAvoidWeight = 1.5f;
    public float obstacleRadius = 0.25f;

    [Header("Boundary")]
    public float boundaryRadius = 5f;
    public float boundaryWeight = 1.0f;

    private Vector3 velocity;
    private float speed;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        velocity = transform.forward * speed;
    }

    void Update()
    {
        if (AquariumManager.Instance == null || AquariumManager.Instance.center == null)
            return;

        List<FishFlocking> neighbors = GetNeighbors();

        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;

        foreach (var fish in neighbors)
        {
            alignment += fish.velocity.normalized;
            cohesion += fish.transform.position;

            float dist = Vector3.Distance(transform.position, fish.transform.position);
            if (dist < separationDistance && dist > 0.001f)
            {
                separation += (transform.position - fish.transform.position).normalized / dist;
            }
        }

        if (neighbors.Count > 0)
        {
            alignment = (alignment / neighbors.Count).normalized;
            cohesion = ((cohesion / neighbors.Count) - transform.position).normalized;
            separation = separation.normalized;
        }

        Vector3 boundary = GetBoundaryForce();
        Vector3 avoid = GetObstacleAvoidance();

        // Keep some current heading so fish don't orbit in circles
        Vector3 desiredDir =
            velocity.normalized * 1.5f +
            alignment * alignmentWeight +
            cohesion * cohesionWeight +
            separation * separationWeight +
            boundary * boundaryWeight +
            avoid * obstacleAvoidWeight;

        if (desiredDir.sqrMagnitude < 0.001f)
            desiredDir = transform.forward;

        desiredDir.Normalize();

        Vector3 desiredVelocity = desiredDir * speed;

        // Smooth velocity change
        velocity = Vector3.Lerp(velocity, desiredVelocity, acceleration * Time.deltaTime);

        // Limit turning rate
        Vector3 newForward = Vector3.RotateTowards(
            transform.forward,
            velocity.normalized,
            turnSpeed * Time.deltaTime,
            0f
        );

        transform.rotation = Quaternion.LookRotation(newForward);
        velocity = newForward * speed;

        transform.position += velocity * Time.deltaTime;
    }

    Vector3 GetBoundaryForce()
    {
        Vector3 toCenter = AquariumManager.Instance.center.position - transform.position;
        float dist = toCenter.magnitude;

        float t = Mathf.Clamp01(dist / boundaryRadius);
        return toCenter.normalized * (t * t);
    }

    Vector3 GetObstacleAvoidance()
    {
        Vector3 origin = transform.position;

        Vector3[] dirs =
        {
            transform.forward,
            (transform.forward + transform.right * 0.5f).normalized,
            (transform.forward - transform.right * 0.5f).normalized
        };

        Vector3 avoid = Vector3.zero;
        int hits = 0;

        foreach (var dir in dirs)
        {
            if (Physics.SphereCast(origin, obstacleRadius, dir, out RaycastHit hit, obstacleCheckDistance, obstacleLayer))
            {
                hits++;

                // Push away from obstacle surface
                avoid += hit.normal;

                // Only a small sideways correction
                Vector3 side = Vector3.Cross(hit.normal, Vector3.up).normalized;
                if (Vector3.Dot(side, transform.right) < 0f)
                    side = -side;

                avoid += side * 0.15f;
            }
        }

        if (hits == 0)
            return Vector3.zero;

        avoid /= hits;
        avoid.y *= 0.3f;
        return avoid.normalized;
    }

    List<FishFlocking> GetNeighbors()
    {
        List<FishFlocking> neighbors = new List<FishFlocking>();

        foreach (var fish in allFish)
        {
            if (fish == this) continue;

            if (Vector3.Distance(transform.position, fish.transform.position) < neighborRadius)
                neighbors.Add(fish);
        }

        return neighbors;
    }
}