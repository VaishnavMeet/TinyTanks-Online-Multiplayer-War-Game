using System.Collections;
using UnityEngine;

public class HideTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestoryTree());
    }

    IEnumerator DestoryTree()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
}
