using System.Collections;
using UnityEngine;
using Photon.Pun;

public class OilTank : MonoBehaviourPun
{
    private bool isOiled = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView view = collision.GetComponent<PhotonView>();

        // Ensure only the owning player triggers the effect
        if (collision.CompareTag("Player") && view != null && view.IsMine && !isOiled)
        {
            TankController2D player = collision.GetComponent<TankController2D>();
            if (player != null)
            {
                isOiled = true;
                player.moveSpeed = 5;
                StartCoroutine(RestartSpeed(player));
            }
        }
    }

    private IEnumerator RestartSpeed(TankController2D player)
    {
        Debug.Log("Speed boosted: " + player.moveSpeed);
        yield return new WaitForSeconds(15);
        player.moveSpeed = 3;
        Debug.Log("Speed reset: " + player.moveSpeed);

        // Destroy the oil tank pickup across the network
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
