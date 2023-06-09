using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float explosionRadius = 3f; // Radius of the explosion when the enemy gets too near
    public int damage = 10; // Damage inflicted on the player when the enemy explodes

    private float health = 100f;

    private Transform player; // Reference to the player's transform
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        Debug.Log("aa");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log(player.position);
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        navMeshAgent.SetDestination(player.position);
    }

    private void Update()
    {
        // Check the distance to the player
        navMeshAgent.SetDestination(player.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= explosionRadius)
        {
            // Player is within explosion radius, trigger explosion
            Explode();
        }
    }

    private void Explode()
    {
        // // Apply damage to the player
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        // if (playerHealth != null)
        // {
        //     playerHealth.TakeDamage(damage);
        // }

        // // Create an explosion effect at the enemy's position
        // GameObject explosion = new GameObject("Explosion");
        // explosion.transform.position = transform.position;

        // // Add a sphere collider to the explosion object to detect nearby entities
        // SphereCollider explosionCollider = explosion.AddComponent<SphereCollider>();
        // explosionCollider.radius = explosionRadius;

        // // Check if any entities are within the explosion radius and apply damage to them
        // Collider[] colliders = Physics.OverlapSphere(explosion.transform.position, explosionRadius);
        // foreach (Collider collider in colliders)
        // {
        //     // Apply damage to the player and any other entities in range
        //     PlayerHealth health = collider.GetComponent<PlayerHealth>();
        //     if (health != null)
        //     {
        //         health.TakeDamage(damage);
        //     }
        // }

        // // Destroy the enemy and the explosion object
        // Destroy(gameObject);
        // Destroy(explosion, 0.1f); // Delay the destruction of the explosion object slightly to allow for collision detection
    }

    public void TakeDamage(float damage) {
        Debug.Log(health);
        health -= damage;
        if(health <= 0){
            Destroy(gameObject);
        }
    }
}