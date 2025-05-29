using System.Collections;
using UnityEngine;

public class OilTank : MonoBehaviour
{
    bool isOiled = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOiled)
        {
            TankController2D player = collision.GetComponent<TankController2D>();
            player.moveSpeed = 5;
            isOiled = true;
            StartCoroutine(RestartSpeed(player));
        }
    }

    public IEnumerator RestartSpeed(TankController2D player)
    {
        Debug.Log(player.moveSpeed);
        yield return new WaitForSeconds(15);
        player.moveSpeed = 3;
        Debug.Log(player.moveSpeed);
        Destroy(gameObject);
    }
}
