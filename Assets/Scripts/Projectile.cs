using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 35f; // Speed of the bullet
    public int maxPiercingCount = 0; // Maximum number of enemies the bullet can pierce (0 for no piercing)
    public bool explodeAtEnd = false; // Should the bullet explode at the end of its trajectory?
    public float explosionRadius = 5f; // Radius of the explosion if explodeAtEnd is true
    public float lifeTime = 10f;
    private float timer;

    public float pierceDamage = 10;
    public float explosiveDamage = 0;

    private int piercingCount = 0; // Current piercing count
    private Rigidbody rb;
    private AudioSource hitSound;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Set the initial velocity of the bullet
        rb.velocity = transform.forward * speed;
        hitSound = GetComponent<AudioSource>();
    }

    private void CreateSound(){
        GameObject soundGO = new GameObject("Sound");
        AudioSource soundAudioSource = soundGO.AddComponent<AudioSource>();
        soundAudioSource.clip = hitSound.clip;
        soundAudioSource.playOnAwake = true;
        soundGO.AddComponent<ImpactSound>();
        Instantiate(soundGO, gameObject.transform.position, gameObject.transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet has hit an enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Apply damage to the enemy
            enemy.TakeDamage(pierceDamage);
            CreateSound();

            // Increment the piercing count and check if the bullet can still pierce
            piercingCount++;
            if (piercingCount > maxPiercingCount)
            {
                // Destroy the bullet if it has reached the maximum piercing count
                if (explodeAtEnd)
                {
                    Explode();
                }
                else {
                    enemy.TakeDamage(pierceDamage);
                    Destroy(gameObject);
                }
            }
        }
        
    }

    private void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if the bullet has reached its lifetime
        if (timer >= lifeTime)
        {
            // Destroy the bullet game object
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude <= 0.001f)
        {
            if(explodeAtEnd){
                // Explode();
            } else {
                Destroy(gameObject);
            }
            
        }
    }

    private void Explode()
    {
        // Create an explosion effect at the current position
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        // Add a sphere collider to the explosion object to detect nearby enemies
        SphereCollider explosionCollider = explosion.AddComponent<SphereCollider>();
        explosionCollider.radius = explosionRadius;

        // Check if any enemies are within the explosion radius and apply damage to them
        Collider[] colliders = Physics.OverlapSphere(explosion.transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosiveDamage);
            }
        }

        // Destroy the bullet and the explosion object
        Destroy(gameObject);
        Destroy(explosion, 0.1f); // Delay the destruction of the explosion object slightly to allow for the collision detection
    }
}