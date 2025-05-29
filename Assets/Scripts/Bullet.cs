using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 4f;
    public float lifeTime = 1f;
    public float damage ;
    private Rigidbody2D rb;
    public Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Move the bullet forward in its facing direction (up)
        rb.linearVelocity = transform.up * speed;

      
    }

    

    // Use this if your bullet has a non-trigger collider
    void OnCollisionEnter2D(Collision2D collision)
    {
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
