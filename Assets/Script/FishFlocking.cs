using System.Collections.Generic;
using UnityEngine;

public class FishFlocking : MonoBehaviour
{
    // ===== STATIC LIST (OPTIMIZED) =====
    public static List<FishFlocking> allFish = new List<FishFlocking>();

    void OnEnable() => allFish.Add(this);
    void OnDisable() => allFish.Remove(this);

    // ===== MOVEMENT SETTINGS =====
    public float speed = 2f;
    public float neighborRadius = 3f;
    public float separationDistance = 1f;

    // ===== FLOCKING WEIGHTS =====
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;

    // ===== OBSTACLE AVOIDANCE =====
    public float obstacleAvoidDistance = 2f;
    public float obstacleAvoidStrength = 5f;
    public LayerMask obstacleLayer;

    // ===== BOUNDARY SETTINGS =====
    public Vector3 aquariumCenter = Vector3.zero;
    public float boundaryRadius = 10f;
    public float boundaryForce = 2f;

    // ===== INTERNAL =====
    private Vector3 velocity;

    void Start()
    {
        velocity = transform.forward * speed;

        // Optional randomness (makes fish less robotic)
        speed = Random.Range(1.5f, 3f);
    }

    void Update()
    {
        List<FishFlocking> neighbors = GetNeighbors();

        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;

        foreach (var fish in neighbors)
        {
            alignment += fish.velocity;
            cohesion += fish.transform.position;

            float dist = Vector3.Distance(transform.position, fish.transform.position);

            if (dist < separationDistance)
            {
                separation += (transform.position - fish.transform.position) / dist;
            }
        }

        if (neighbors.Count > 0)
        {
            alignment /= neighbors.Count;
            cohesion = (cohesion / neighbors.Count) - transform.position;
        }

        // ===== OBSTACLE AVOIDANCE =====
        Vector3 avoidDir = Vector3.zero;
        RaycastHit hit;

        Vector3[] directions = {
            transform.forward,
            Quaternion.AngleAxis(-30, Vector3.up) * transform.forward,
            Quaternion.AngleAxis(30, Vector3.up) * transform.forward
        };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out hit, obstacleAvoidDistance, obstacleLayer))
            {
                avoidDir += hit.normal * obstacleAvoidStrength;
            }
        }

        // ===== BOUNDARY FORCE =====
        Vector3 toCenter = aquariumCenter - transform.position;
        Vector3 boundaryDir = Vector3.zero;

        if (toCenter.magnitude > boundaryRadius)
        {
            boundaryDir = toCenter.normalized * boundaryForce;
        }

        // ===== COMBINE ALL FORCES =====
        Vector3 acceleration =
            alignment * alignmentWeight +
            cohesion * cohesionWeight +
            separation * separationWeight +
            avoidDir +
            boundaryDir;

        // ===== APPLY MOVEMENT =====
        velocity += acceleration * Time.deltaTime;

        // Prevent crazy speeds
        velocity = velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity;

        // Small randomness to avoid stuck behavior
        velocity += Random.insideUnitSphere * 0.1f;
    }

    List<FishFlocking> GetNeighbors()
    {
        List<FishFlocking> neighbors = new List<FishFlocking>();

        foreach (var fish in allFish)
        {
            if (fish == this) continue;

            if (Vector3.Distance(transform.position, fish.transform.position) < neighborRadius)
            {
                neighbors.Add(fish);
            }
        }

        return neighbors;
    }
}