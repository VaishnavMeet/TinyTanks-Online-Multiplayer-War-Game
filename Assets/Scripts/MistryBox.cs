using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class MistryBox : MonoBehaviourPunCallbacks
{
    public List<GameObject> players;
    private int playerNo;
    private TankSwitcher tankSwitcher;

    void Start()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        if (gameManager != null)
            tankSwitcher = gameManager.GetComponent<TankSwitcher>();

        playerNo = Random.Range(0, players.Count);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView view = collision.GetComponent<PhotonView>();
        if (view != null && view.IsMine && tankSwitcher != null)
        {
            tankSwitcher.SwitchTank(players[playerNo]);

            // Ask MasterClient to destroy this box
            photonView.RPC("DestroyBox", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void DestroyBox()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
