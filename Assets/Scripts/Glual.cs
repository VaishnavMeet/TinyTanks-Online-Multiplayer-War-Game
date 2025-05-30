using System.Collections;
using UnityEngine;

public class Glual : MonoBehaviour
{
    public float health;
    public float maxHealth = 200;

    void Start()
    {
        health = 200f;
        StartCoroutine(DestoryTheBullte());    
    }
    public void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestoryTheBullte()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    } 
}
