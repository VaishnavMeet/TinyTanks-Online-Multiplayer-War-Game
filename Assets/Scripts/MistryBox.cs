using UnityEngine;
using System.Collections.Generic;

public class MistryBox : MonoBehaviour
{
    GameObject gamemanager;
    public List<GameObject> players; // List of all tank prefabs
    private int playerNo;

    void Start()
    {
       gamemanager= GameObject.FindWithTag("GameManager");
        playerNo = Random.Range(0, players.Count);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has a TankSwitcher component
        TankSwitcher switcher = gamemanager.GetComponent<TankSwitcher>();
        if (switcher != null)
        {
            // Call SwitchTank with a random player prefab
            switcher.SwitchTank(players[0]);

            // Optional: destroy or deactivate the box after use
            Destroy(gameObject);
        }
    }
}
