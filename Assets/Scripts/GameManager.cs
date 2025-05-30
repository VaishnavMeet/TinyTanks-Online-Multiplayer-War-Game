using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public List<Transform> spawneGeneration; // Spawn points
    public List<Transform> powerspawneGeneration; // Spawn points
    public GameObject BulletPickupPrefab;    // Prefab to instantiate at each point
    public List<GameObject> allBulletPrefab; // All bullet/item prefabs to assign randomly
    public GameObject PowerPickupPrefab;
    public List<GameObject> allPowerPrefab;

    public List<Sprite> allTankBodySprites;
    public List<Sprite> allTankSmallBarrelSprites;
    public List<Sprite> allTankMediumBarrelSprites;
    public List<Sprite> allTankBigSprites;

    public GameObject GlualUi;
    public GameObject AiUi ;
    public GameObject TreeUi ;
    public GameObject ObstclesUi ;
    public GameObject SpeedUi ;

    private List<GameObject> currentPickups = new List<GameObject>();
    private List<GameObject> currentPowerPickups = new List<GameObject>();

    [Header("Scope")]
    int ScopeAmount = 5;
    public Camera cam;

    void Start()
    {
        // Spawn Player at random position
        int spawnIndex = Random.Range(0, spawneGeneration.Count);
        Vector3 spawnPos = spawneGeneration[spawnIndex].position;



        GameObject spawnedPlayer = Instantiate(Player, spawnPos, Quaternion.identity);
        GetComponent<TankSwitcher>().currentTank = spawnedPlayer;

        // Now use spawnedPlayer instead of Player
        TankController2D controller = spawnedPlayer.GetComponent<TankController2D>();

        Sprite barrel;
        int rand = Random.Range(0, allTankBodySprites.Count);
        int rand2=Random.Range(0, 3);
        if (rand2 == 1)
        {
            barrel = allTankSmallBarrelSprites[rand];
        }
        if (rand2 == 2)
        {
            barrel = allTankMediumBarrelSprites[rand];
        }
        else
        {
            barrel = allTankBigSprites[rand];
        }


        controller.TankBody.GetComponent<SpriteRenderer>().sprite = allTankBodySprites[rand];
        controller.BarrelBody.GetComponent<SpriteRenderer>().sprite = barrel;
        controller.moveJoystick = GetComponent<TankSwitcher>().moveJoystick;
        controller.aimJoystick = GetComponent<TankSwitcher>().aimJoystick;
        controller.swapeImage = GameObject.FindWithTag("pick").GetComponent<Image>();

         GlualUi= GameObject.FindWithTag("GlualUi");
        AiUi = GameObject.FindWithTag("AiUi");
        TreeUi = GameObject.FindWithTag("TreeUi");
        ObstclesUi = GameObject.FindWithTag("ObstclesUi");
        SpeedUi = GameObject.FindWithTag("SpeedUi");

        controller.GlualTxt = GlualUi.GetComponentInChildren<Text>();
        controller.AiRobotsTxt= AiUi.GetComponentInChildren<Text>();
        controller.TreeHideTxt = TreeUi.GetComponentInChildren<Text>();
        controller.obstclesTxt = ObstclesUi.GetComponentInChildren<Text>();
        controller.SpeedBoastTxt = SpeedUi.GetComponentInChildren<Text>();

        StartCoroutine(DelayedSetup(controller));

        StartCoroutine(SpawnBulletPickupLoop());
        StartCoroutine(SpawnPowerPickupLoop());
    }

    IEnumerator DelayedSetup(TankController2D controller)
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure UI is ready
        Button button = GameObject.FindWithTag("pick")?.GetComponent<Button>();
        Button button1 = GlualUi.GetComponent<Button>();
        Button button2 = AiUi.GetComponent<Button>();
        Button button3 = TreeUi.GetComponent<Button>();
        Button button4 = ObstclesUi.GetComponent<Button>();
        Button button5 = SpeedUi.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(controller.OnSwapButtonClicked);
            button1.onClick.RemoveAllListeners();
            button1.onClick.AddListener(controller.UseGlual);
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(controller.UseAiRobots);
            button3.onClick.RemoveAllListeners();
            button3.onClick.AddListener(controller.UseTreeHide);
            button4.onClick.RemoveAllListeners();
            button4.onClick.AddListener(controller.UseObstacles);
            button5.onClick.RemoveAllListeners();
            button5.onClick.AddListener(controller.UseSpeedBooster);
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
    public void OnClickCamera()
    {
        if (ScopeAmount < 7)
        {
            ScopeAmount++;
            cam.orthographicSize = ScopeAmount;
        }
        else
        {
            ScopeAmount = 5;
            cam.orthographicSize = ScopeAmount;
        }
    }
    IEnumerator SpawnPowerPickupLoop()
    {
        while (true)
        {
            // Wait 20 seconds before spawning
            yield return new WaitForSeconds(25f);

            // Spawn pickups at each spawn point
            foreach (Transform spawnPoint in powerspawneGeneration)
            {
                GameObject pickup = Instantiate(PowerPickupPrefab, spawnPoint.position, Quaternion.identity);

                // Set a random bullet prefab to the pickup
                PowerStore PowerScript = pickup.GetComponent<PowerStore>();
                PowerScript.prefab = allPowerPrefab[Random.Range(0, allPowerPrefab.Count)];

                currentPowerPickups.Add(pickup);
            }

            // Wait 30 seconds, then destroy pickups
            yield return new WaitForSeconds(30f);
            foreach (GameObject pickup in currentPowerPickups)
            {
                if (pickup != null)
                    Destroy(pickup);
            }
            currentPowerPickups.Clear();

            // Wait another 20 seconds before next spawn cycle
            yield return new WaitForSeconds(20f);
        }
    }
}
