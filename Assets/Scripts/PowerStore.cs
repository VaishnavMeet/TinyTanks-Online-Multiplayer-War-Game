using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PowerStore : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public GameObject prefab;
    public List<GameObject> allPowerPrefab; // Assign this via inspector or singleton

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        int index = (int)photonView.InstantiationData[0];
        prefab = allPowerPrefab[index];

        SpriteRenderer prefabSprite = prefab.GetComponentInChildren<SpriteRenderer>();
        GetComponent<SpriteRenderer>().sprite = prefabSprite != null ? prefabSprite.sprite : prefab.GetComponent<SpriteRenderer>().sprite;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TankController2D player = collision.GetComponent<TankController2D>();
            if (prefab.tag == "Glual") player.Glual++;
            if (prefab.tag == "Ai") player.AiRobots++;
            if (prefab.tag == "TreeUi") player.TreeHide++;
            if (prefab.tag == "ObstclesUi") player.obstcles++;
            if (prefab.tag == "SpeedUi") player.SpeedBoast++;
            player.UpdateText();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
