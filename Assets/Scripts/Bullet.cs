using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 1f;
    public float lifeTime = 1f;
    public float reloadingTimeout = 1f;
    public float damage;
    private Rigidbody2D rb;
    public Animator animator;
    AudioSource audioSource;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("Bullet has no Rigidbody2D!");
            return;
        }

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.linearVelocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage Player
        TankController2D player = collision.gameObject.GetComponentInParent<TankController2D>();
        if (player != null)
        {
            player.health -= damage;
        }

        // Damage AI
        AIEnemyShooter ai = collision.gameObject.GetComponentInParent<AIEnemyShooter>();
        if (ai != null)
        {
            ai.health -= damage;
        }

        // Damage Glual
        Glual glual = collision.gameObject.GetComponentInParent<Glual>();
        if (glual != null)
        {
            glual.health -= damage;
        }

        // Play hit sound
        audioSource.enabled = true;

        // Stop bullet movement & physics
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable collider to prevent further collisions
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
            collider.enabled = false;

        // Play explosion animation
        if (animator != null)
            animator.SetBool("Exposion", true);

        // Start delayed destroy coroutine
        StartCoroutine(DestroyTheBullet());
    }

    IEnumerator DestroyTheBullet()
    {
        yield return new WaitForSeconds(0.3f);

        // Use PhotonNetwork.Destroy so all clients remove bullet
        if (photonView.IsMine)
        {
            Photon.Pun.PhotonNetwork.Destroy(gameObject);
        }
    }
}
