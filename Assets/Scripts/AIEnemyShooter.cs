using UnityEngine;
using System.Collections;

public class AIEnemyShooter : MonoBehaviour
{
    public float health = 50f;
    public GameObject bulletPrefab;
    public GameObject bulletFlamsh;
    public Transform firePoint;           // FirePoint (child of body)
    public Transform robotBody;           // Body to rotate (AI_Body)
    public float shootRange = 5f;
    public float shootInterval = 1.5f;
    public float bulletSpeed = 10f;
    public LayerMask playerLayer;         // Assign "Player" layer
    public LayerMask obstacleMask;        // Assign obstacles (walls, etc.)

    private void Start()
    {
        StartCoroutine(ShootAtVisiblePlayer());
    }

    public void Update()
    {
        if (health<=0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ShootAtVisiblePlayer()
    {
        while (true)
        {
            GameObject nearestPlayer = FindVisiblePlayer();

            if (nearestPlayer != null)
            {
                Vector2 direction = (nearestPlayer.transform.position - firePoint.position).normalized;

                // Rotate body toward the target
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                robotBody.rotation = Quaternion.Euler(0, 0, angle+90);

                // Fire the bullet
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, robotBody.eulerAngles.z + 180f));
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.linearVelocity = direction * bulletSpeed;
                bulletFlamsh.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                bulletFlamsh.SetActive(true);

            }

            yield return new WaitForSeconds(shootInterval-0.2f);
        }
    }

    GameObject FindVisiblePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = shootRange;
        GameObject visiblePlayer = null;

        foreach (GameObject player in players)
        {
            Vector2 dir = (player.transform.position - firePoint.position).normalized;
            float dist = Vector2.Distance(firePoint.position, player.transform.position);

            // Check line of sight
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, dist, obstacleMask | playerLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player") && dist < minDistance)
            {
                visiblePlayer = player;
                minDistance = dist;
            }
        }

        return visiblePlayer;
    }
}
