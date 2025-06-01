using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject MistryBox;
    public GameObject OilBox;

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
    GameObject spawnedPlayer;
    private List<GameObject> currentPickups = new List<GameObject>();
    private List<GameObject> currentPowerPickups = new List<GameObject>();
    private List<GameObject> currentBoxPickups = new List<GameObject>();

    [Header("Scope")]
    int ScopeAmount = 5;
    public Camera cam;
    int maxLimit = 5;


    void Start()
    {
        // Spawn Player at random position
        SpawnPlayer();
        StartCoroutine(SpawnBulletPickupLoop());
        StartCoroutine(SpawnBoxPickupLoop());
        StartCoroutine(SpawnPowerPickupLoop());
    }

    void SpawnPlayer()
    {

        int spawnIndex = Random.Range(0, spawneGeneration.Count);
        Vector3 spawnPos = spawneGeneration[spawnIndex].position;
        spawnedPlayer = PhotonNetwork.Instantiate(Player.name, spawnPos, Quaternion.identity);
        cam.GetComponent<CemraSetup>().player = spawnedPlayer.transform;
        SetupPlayer(spawnedPlayer);
    }

    public override void OnJoinedRoom()
    {
        if (spawnedPlayer == null)
            SpawnPlayer();
    }
   

    void SetupPlayer(GameObject player)
    {
        int rand = Random.Range(0, allTankBodySprites.Count);
        int rand2 = Random.Range(0, 3); // barrel type: 0-small, 1-medium, 2-big
        int barrelIndex = rand; // match index with body for simplicity

        TankController2D controller = player.GetComponent<TankController2D>();

        // Call RPC to set sprite on all clients
        player.GetComponent<PhotonView>().RPC("SetTankSkin", RpcTarget.AllBuffered, rand, rand2, barrelIndex);

        // rest of the setup (joystick, UI) remains local only for local player

        maxLimit += rand2;


        if (player.GetComponent<PhotonView>().IsMine)
        {


            GetComponent<TankSwitcher>().currentTank=player;
            controller.moveJoystick = GetComponent<TankSwitcher>().moveJoystick;
            controller.aimJoystick = GetComponent<TankSwitcher>().aimJoystick;
            controller.swapeImage = GameObject.FindWithTag("pick").GetComponent<Image>();

            GlualUi = GameObject.FindWithTag("GlualUi");
            AiUi = GameObject.FindWithTag("AiUi");
            TreeUi = GameObject.FindWithTag("TreeUi");
            ObstclesUi = GameObject.FindWithTag("ObstclesUi");
            SpeedUi = GameObject.FindWithTag("SpeedUi");

            controller.GlualTxt = GlualUi.GetComponentInChildren<Text>();
            controller.AiRobotsTxt = AiUi.GetComponentInChildren<Text>();
            controller.TreeHideTxt = TreeUi.GetComponentInChildren<Text>();
            controller.obstclesTxt = ObstclesUi.GetComponentInChildren<Text>();
            controller.SpeedBoastTxt = SpeedUi.GetComponentInChildren<Text>();

            StartCoroutine(DelayedSetup(controller));
        }
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



    
    
    

    public void OnPlayerDeath()
    {
        StartCoroutine(RespawnAfterDelay(3f));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        int spawnIndex = Random.Range(0, spawneGeneration.Count);
        Vector3 spawnPos = spawneGeneration[spawnIndex].position;

        spawnedPlayer = PhotonNetwork.Instantiate(Player.name, spawnPos, Quaternion.identity);
        SetupPlayer(spawnedPlayer);
    }

    public void OnClickCamera()
    {
        if (ScopeAmount < maxLimit)
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
            yield return new WaitForSeconds(25f);

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Transform spawnPoint in powerspawneGeneration)
                {
                    int randomPowerIndex = Random.Range(0, allPowerPrefab.Count);
                    object[] instantiationData = new object[] { randomPowerIndex };

                    GameObject pickup = PhotonNetwork.Instantiate(PowerPickupPrefab.name, spawnPoint.position, Quaternion.identity, 0, instantiationData);


                    currentPowerPickups.Add(pickup);
                }

                yield return new WaitForSeconds(30f);
                foreach (GameObject pickup in currentPowerPickups)
                {
                    if (pickup != null)
                        PhotonNetwork.Destroy(pickup); // Use PhotonNetwork.Destroy
                }
                currentPowerPickups.Clear();
            }

            yield return new WaitForSeconds(20f);
        }
    }
    IEnumerator SpawnBoxPickupLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f); // Initial wait before spawning

            if (PhotonNetwork.IsMasterClient)
            {
                // Pick 3 unique random spawn points
                List<Transform> availableSpawns = new List<Transform>(spawneGeneration);
                for (int i = 0; i < 6 && availableSpawns.Count > 0; i++)
                {
                    int randSpawnIndex = Random.Range(0, availableSpawns.Count);
                    Transform spawnPoint = availableSpawns[randSpawnIndex];
                    availableSpawns.RemoveAt(randSpawnIndex); // Ensure uniqueness

                    // Randomly choose between MistryBox and OilBox
                    GameObject boxPrefab = i % 2 == 1 ? MistryBox : OilBox;

                    GameObject box = PhotonNetwork.Instantiate(boxPrefab.name, spawnPoint.position, Quaternion.identity);
                    currentBoxPickups.Add(box);
                }

                yield return new WaitForSeconds(50f); // How long boxes remain in scene

                foreach (GameObject box in currentBoxPickups)
                {
                    if (box != null)
                        PhotonNetwork.Destroy(box);
                }
                currentBoxPickups.Clear();
            }

            yield return new WaitForSeconds(10f); // Delay before next spawn cycle
        }
    }



    IEnumerator SpawnBulletPickupLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Transform spawnPoint in spawneGeneration)
                {
                    int randomIndex = Random.Range(0, allBulletPrefab.Count);
                    object[] instantiationData = new object[] { randomIndex };

                    GameObject pickup = PhotonNetwork.Instantiate(BulletPickupPrefab.name, spawnPoint.position, Quaternion.identity, 0, instantiationData);
                    


                    currentPickups.Add(pickup);
                }

                yield return new WaitForSeconds(30f);
                foreach (GameObject pickup in currentPickups)
                {
                    if (pickup != null)
                        PhotonNetwork.Destroy(pickup); // Use PhotonNetwork.Destroy
                }
                currentPickups.Clear();
            }

            yield return new WaitForSeconds(20f);
        }
    }

    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f); // Respawn delay (optional)

        int spawnIndex = Random.Range(0, spawneGeneration.Count);
        Vector3 spawnPos = spawneGeneration[spawnIndex].position;

        GameObject newPlayer = PhotonNetwork.Instantiate(Player.name, spawnPos, Quaternion.identity);
        cam.GetComponent<CemraSetup>().player = newPlayer.transform;

        SetupPlayer(newPlayer);
    }


}

