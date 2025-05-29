using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1f;
    public float lifeTime = 1f;
    public float damage ;
    private Rigidbody2D rb;
    public Animator animator;
    AudioSource audioSource;
    void Start()
    {
        if (GetComponent<Rigidbody2D>()==null) return;
        rb = GetComponent<Rigidbody2D>();

        // Move the bullet forward in its facing direction (up)
        rb.linearVelocity = transform.up * speed;
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject,lifeTime);
    }

    

    // Use this if your bullet has a non-trigger collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<TankController2D>().health -= damage;
            Debug.Log(collision.gameObject.GetComponent<TankController2D>().health);
        }
        audioSource.enabled = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(DestoryTheBullte());
        animator.SetBool("Exposion", true);
    }

    IEnumerator DestoryTheBullte()
    {
        yield return new WaitForSeconds(0.30f);
        Destroy(gameObject);
    }

}
