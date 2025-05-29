using System.Collections;
using UnityEngine;

public class HealthContainer : MonoBehaviour
{
    public float maxHealth = 300f;
    public float health = 300f;
    public float increaseRate = 5f;

    private bool isHealthZoneActive = false;
    private Collider2D currentCollider;
    private Coroutine healthCoroutine;


    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentCollider = collision;
            isHealthZoneActive = true;
            healthCoroutine = StartCoroutine(IncreaseHealthOverTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHealthZoneActive = false;
            if (healthCoroutine != null)
            {
                StopCoroutine(healthCoroutine);
                healthCoroutine = null;
                StartCoroutine(SelfHealing());
            }
        }
    }

    IEnumerator IncreaseHealthOverTime()
    {
        while (isHealthZoneActive && currentCollider != null)
        {
            TankController2D tank = currentCollider.GetComponent<TankController2D>();
            if (tank != null)
            {
                if (tank.maxHealth >= tank.health+increaseRate)
                {
                tank.health += increaseRate;
                health-=increaseRate;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SelfHealing()
    {
        yield return new WaitForSeconds(2f);
        if(health<maxHealth) health += 1;
        StartCoroutine(SelfHealing());
    }
}
