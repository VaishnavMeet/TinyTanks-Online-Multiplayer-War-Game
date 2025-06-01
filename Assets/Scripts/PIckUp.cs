using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public GameObject prefab; // This is the bullet or item prefab
     public List<GameObject> allBulletPrefab;
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int index = (int)photonView.InstantiationData[0];
        prefab = allBulletPrefab[index];

        if (prefab.GetComponentInChildren<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        else
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TankController2D player = collision.GetComponent<TankController2D>();

            // Show image
            player.swapeImage.color = new Color(1, 1, 1, 1); // Fully visible
            player.swapeImage.sprite = GetComponent<SpriteRenderer>().sprite;

            // Store this pickup reference so button knows what to swap
            player.currentPickup = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TankController2D player = collision.GetComponent<TankController2D>();

            // Hide image
            player.swapeImage.color = new Color(1, 1, 1, 0); // Fully transparent
            player.swapeImage.sprite = null;

            // Clear reference
            player.currentPickup = null;
        }
    }
}
