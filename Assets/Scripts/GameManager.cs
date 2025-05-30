using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public List<Transform> spawneGeneration; // Spawn points
    public GameObject BulletPickupPrefab;    // Prefab to instantiate at each point
    public List<GameObject> allBulletPrefab; // All bullet/item prefabs to assign randomly

    public List<Sprite> allTankBodySprites;
    
    private List<GameObject> currentPickups = new List<GameObject>();

    void Start()
    {
        // Spawn Player at random position
        int spawnIndex = Random.Range(0, spawneGeneration.Count);
        Vector3 spawnPos = spawneGeneration[spawnIndex].position;

        GetComponent<TankSwitcher>().currentTank=Instantiate(Player, spawnPos, Quaternion.identity);
        Player.transform.position = spawneGeneration[spawnIndex].position;

        // Assign random barrel and tank sprites
        int rand = Random.Range(0, allTankBodySprites.Count);

       
        Player.GetComponent<TankController2D>().TankBody.GetComponent<SpriteRenderer>().sprite = allTankBodySprites[rand];
        Player.GetComponent<TankController2D>().moveJoystick=GetComponent<TankSwitcher>().moveJoystick;
        Player.GetComponent<TankController2D>().aimJoystick=GetComponent<TankSwitcher>().aimJoystick;

        Player.GetComponent<TankController2D>().swapeImage=GameObject.FindWithTag("pick").GetComponent<Image>();

        StartCoroutine(DelayedSetup());

        // Start bullet spawn cycle 
        StartCoroutine(SpawnBulletPickupLoop());
    }

    IEnumerator DelayedSetup()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure UI is ready
        Button button = GameObject.FindWithTag("pick")?.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => Debug.Log("Button Clicked!"));
            Debug.Log("Listener attached!");
        }
        else
        {
            Debug.LogError("Button not found or missing 'pick' tag!");
        }
    }


    IEnumerator SpawnBulletPickupLoop()
    {
        while (true)
        {
            // Wait 20 seconds before spawning
            yield return new WaitForSeconds(20f);

            // Spawn pickups at each spawn point
            foreach (Transform spawnPoint in spawneGeneration)
            {
                GameObject pickup = Instantiate(BulletPickupPrefab, spawnPoint.position, Quaternion.identity);

                // Set a random bullet prefab to the pickup
                PickUp pickupScript = pickup.GetComponent<PickUp>();
                pickupScript.prefab = allBulletPrefab[Random.Range(0, allBulletPrefab.Count)];

                currentPickups.Add(pickup);
            }

            // Wait 30 seconds, then destroy pickups
            yield return new WaitForSeconds(30f);
            foreach (GameObject pickup in currentPickups)
            {
                if (pickup != null)
                    Destroy(pickup);
            }
            currentPickups.Clear();

            // Wait another 20 seconds before next spawn cycle
            yield return new WaitForSeconds(20f);
        }
    }
}
