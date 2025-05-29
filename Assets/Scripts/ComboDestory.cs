using System.Collections;
using UnityEngine;

public class ComboDestory : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(DestoryTheCombo());
    }

    IEnumerator DestoryTheCombo()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
