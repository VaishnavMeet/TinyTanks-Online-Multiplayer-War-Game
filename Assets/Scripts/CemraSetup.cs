using Unity.Cinemachine;
using UnityEngine;

public class CemraSetup : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Camera stays behind the player
    public float followSpeed = 5f;

    public Transform player;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
