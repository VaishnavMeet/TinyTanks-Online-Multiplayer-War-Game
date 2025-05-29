using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 4f;
    public float lifeTime = 1f;
    public float damage = 10;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Move the bullet forward in its facing direction (up)
        rb.linearVelocity = transform.up * speed;

        // Destroy the bullet after a while
        Destroy(gameObject, lifeTime);
    }

    // Use this if your bullet has a trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision with: " + other.name);
        Destroy(gameObject);
    }

    // Use this if your bullet has a non-trigger collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision with: " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
