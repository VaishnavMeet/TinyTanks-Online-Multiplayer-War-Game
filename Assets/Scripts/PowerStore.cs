using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PowerStore : MonoBehaviour
{
    public GameObject prefab;

    private void Start()
    {
        if (prefab.GetComponentInChildren<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        }
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
            Destroy(gameObject);
        }
    }
}
