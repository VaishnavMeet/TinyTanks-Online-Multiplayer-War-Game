using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankController2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public Transform tankBody;
    public List<Transform> barrels;

    private Rigidbody2D rb;
    private PhotonView view;

    [Header("Skin")]
    public GameObject TankBody;
    public GameObject BarrelBody;

    [Header("Features")]
    public float maxHealth = 200f;
    public float health = 200f;
    public Image swapeImage;
    public PickUp currentPickup;

    [Header("Powers")]
    public GameObject GlualPrefab;
    public GameObject AiRobotsPrefab;
    public GameObject obstclesPrefab;
    public GameObject TreeHidePrefab;
    public GameObject SpeedBoastPrefab;
    public int Glual = 1, AiRobots = 1, obstcles = 1, TreeHide = 1, SpeedBoast = 1;
    public Text GlualTxt, AiRobotsTxt, obstclesTxt, TreeHideTxt, SpeedBoastTxt;

    [Header("Firing Assets")]
    public GameObject Bullet;
    public List<Transform> firePoints;
    public List<GameObject> FireFlams;
    private bool isFire = false, isReloading = false, isFiveShot = false;
    public float timeout = 1f;
    private string currentBulletName;


    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip ride, fire;

    private void Start()
    {
        currentBulletName = Bullet.name;
        view = GetComponent<PhotonView>();
        swapeImage = GameObject.FindWithTag("pick")?.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!view.IsMine) return;

        HandleMovement();
        HandleBarrelRotation();

        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        if (view.IsMine)
        {
            FindObjectOfType<GameManager>().StartCoroutine("RespawnPlayer");
        }

        PhotonNetwork.Destroy(gameObject); // Remove tank across all clients
        yield return null;
    }


    private void HandleMovement()
    {
        Vector2 moveInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 direction = moveInput.normalized;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * moveSpeed, Time.deltaTime * 10f);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            tankBody.rotation = Quaternion.Slerp(tankBody.rotation, targetRotation, Time.deltaTime * 8f);

            view.RPC("SyncTankBodyRotation", RpcTarget.Others, angle);

            if (!audioSource.isPlaying) audioSource.PlayOneShot(ride);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (audioSource.isPlaying) audioSource.Pause();
        }
    }

    [PunRPC]
    void SyncTankBodyRotation(float angle)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        tankBody.rotation = Quaternion.Slerp(tankBody.rotation, targetRotation, Time.deltaTime * 8f);
    }

    private void HandleBarrelRotation()
    {
        Vector2 aimInput = new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);
        float magnitude = aimInput.magnitude;

        if (magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg + 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            foreach (Transform barrel in barrels)
            {
                barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotation, Time.deltaTime * 15f);
            }

            view.RPC("SyncBarrelRotation", RpcTarget.Others, angle);

            if (magnitude >= 0.95f && !isFire && !isReloading)
            {
                isFire = true;
                timeout = Bullet.GetComponent<Bullet>().reloadingTimeout;
                isFiveShot = Bullet.GetComponent<FiveShotGeneration>() != null;

                StartCoroutine(ShootAndReload(timeout));
            }
        }
    }

    [PunRPC]
    void SyncBarrelRotation(float angle)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        foreach (Transform barrel in barrels)
        {
            barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    private IEnumerator ShootAndReload(float timeout)
    {
        if (isFiveShot)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return StartCoroutine(SingleShoot());
            }
        }
        else
        {
            yield return StartCoroutine(SingleShoot());
        }

        yield return new WaitForSeconds(timeout);
        isReloading = false;
    }

    private IEnumerator SingleShoot()
    {
        isReloading = true;
        foreach (var flame in FireFlams) flame.SetActive(true);

        foreach (Transform firePoint in firePoints)
        {
            PhotonNetwork.Instantiate(currentBulletName, firePoint.position, Quaternion.Euler(0, 0, firePoint.eulerAngles.z + 180f));
            if (fire != null) audioSource.PlayOneShot(fire);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);
        foreach (var flame in FireFlams) flame.SetActive(false);
        isFire = false;
    }

    public void OnSwapButtonClicked()
    {
        view.RPC("RPC_SwitchBullet", RpcTarget.AllBuffered);
    }


    [PunRPC]
    void RPC_SwitchBullet()
    {
        if (currentPickup == null) return;

        GameObject newBullet = currentPickup.prefab;
        string newBulletName = newBullet.name;

        // Swap locally
        GameObject temp = Bullet;
        Bullet = newBullet;
        currentPickup.prefab = temp;
        currentBulletName = newBulletName;
        Debug.Log(currentPickup.prefab.name);
        // Update UI
        Sprite newSprite = currentPickup.prefab.GetComponentInChildren<SpriteRenderer>()?.sprite ??
                           currentPickup.prefab.GetComponent<SpriteRenderer>()?.sprite;

        if (newSprite != null)
        {
            swapeImage.sprite = newSprite;
            currentPickup.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }



    public void UseGlual() { UseUtility(ref Glual, GlualPrefab); }
    public void UseAiRobots() { UseUtility(ref AiRobots, AiRobotsPrefab); }
    public void UseObstacles() { UseUtility(ref obstcles, obstclesPrefab); }
    public void UseTreeHide() { UseUtility(ref TreeHide, TreeHidePrefab); }
    public void UseSpeedBooster()
    {
        if (SpeedBoast > 0)
        {
            StartCoroutine(SpeedBoost());
            SpeedBoast--;
            UpdateText();
        }
    }

    void UseUtility(ref int count, GameObject prefab)
    {
        if (count > 0)
        {
            PhotonNetwork.Instantiate(prefab.name, firePoints[0].position, Quaternion.identity);
            count--;
            UpdateText();
        }
    }

    IEnumerator SpeedBoost()
    {
        moveSpeed = 5;
        yield return new WaitForSeconds(10f);
        moveSpeed = 3;
    }

    [PunRPC]
    public void SetTankSkin(int bodyIndex, int barrelType, int barrelIndex)
    {
        var gm = FindObjectOfType<GameManager>();
        TankBody.GetComponent<SpriteRenderer>().sprite = gm.allTankBodySprites[bodyIndex];
        BarrelBody.GetComponent<SpriteRenderer>().sprite = barrelType switch
        {
            0 => gm.allTankSmallBarrelSprites[barrelIndex],
            1 => gm.allTankMediumBarrelSprites[barrelIndex],
            _ => gm.allTankBigSprites[barrelIndex],
        };
    }

    public void UpdateText()
    {
        GlualTxt.text = Glual.ToString();
        AiRobotsTxt.text = AiRobots.ToString();
        TreeHideTxt.text = TreeHide.ToString();
        SpeedBoastTxt.text = SpeedBoast.ToString();
        obstclesTxt.text = obstcles.ToString();
    }
}
