using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class TankController2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;

    public Joystick moveJoystick; // Right joystick
    public Joystick aimJoystick;  // Left joystick

    public Transform tankBody;     // Reference to TankBody
    public List<Transform> barrels; // Drag Barrel1, Barrel2, Barrel3 here in Inspector

    private Rigidbody2D rb;

    [Header("Skin")]
    public GameObject TankBody;
    public GameObject BarrelBody;

    [Header("Feactures")]
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
    public int Glual = 1;
    public int AiRobots = 1;
    public int obstcles = 1;
    public int TreeHide = 1;
    public int SpeedBoast = 1;
    public Text GlualTxt;
    public Text AiRobotsTxt;
    public Text obstclesTxt;
    public Text TreeHideTxt;
    public Text SpeedBoastTxt;

    [Header("Firing Assests")]
    public GameObject Bullet;
    public List<Transform> firePoints;
    public List<GameObject> FireFlams;
    public bool isFire = false;
    public bool isReloading = false;
    public float timeout = 1f;
    bool isFiveShot = false;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip ride;
    public AudioClip fire;
    bool wasPaused = false;

    
    
    void Start()
    {
        swapeImage = GameObject.FindWithTag("pick").GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        HandleMovement();
        HandleBarrelRotation();
    }

    void HandleMovement()
    {
        Vector2 moveInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 moveDirection = moveInput.normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
            if (!audioSource.isPlaying)
            {
                 audioSource.Play();
            }
            // Rotate tank body to face movement direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg ;
            angle += 90;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            tankBody.rotation = Quaternion.RotateTowards(tankBody.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            audioSource.Pause();
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleBarrelRotation()
    {
        Vector2 aimInput = new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);
        float magnitude = aimInput.magnitude;

        if (magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg + 90f;
            foreach (Transform barrel in barrels)
            {
                barrel.rotation = Quaternion.Euler(0, 0, angle);
            }
            // Fire if joystick is pushed to its limit (or very close)
            if (magnitude >= 0.95f) // adjust threshold if needed
            {
                if (!isFire && !isReloading)
                {
                    isFire= true;
                    if (audioSource.isPlaying)
                    {
                       
                    }
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(fire);                        
                    }
                    timeout = Bullet.GetComponent<Bullet>().reloadingTimeout;
                    if(Bullet.GetComponent<FiveShotGeneration>()!=null)
                    {
                        isFiveShot= true ;
                    }
                    else
                    {
                        isFiveShot = false ;
                    }
                    StartCoroutine(ShootAndRealoding(timeout));
                }
                
            }
        }
    }

    IEnumerator ShootAndRealoding(float timeout)
    {
        if (isFiveShot)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return StartCoroutine(SingleShoot()); // Wait for each shot to finish

            }
        }
        else
        {
            StartCoroutine(SingleShoot());
        }


        yield return new WaitForSeconds(timeout);
        isReloading= false;
    }

    IEnumerator SingleShoot()
    {
        isFire = false;
        isReloading = true;
        foreach (var flame in FireFlams)
        {
            flame.SetActive(true);
        }

        for (int i = 0; i < firePoints.Count; i++)
        {
            Transform firePoint = firePoints[i];
            GameObject bullet = Instantiate(
                Bullet,
                firePoint.position,
                Quaternion.Euler(0, 0, firePoint.eulerAngles.z + 180f)
            );
            yield return new WaitForSeconds(0.1f);
        }

        //yield return new WaitForSeconds(0.1f);

        // Disable all flames
        foreach (var flame in FireFlams)
        {
            yield return new WaitForSeconds(0.1f);
            flame.SetActive(false);
        }
    }


    public void OnSwapButtonClicked()
    {
        if (currentPickup != null)
        {
            GameObject temp= Bullet;
            Bullet = currentPickup.prefab;
            currentPickup.prefab = temp;
            if (currentPickup.prefab.GetComponentInChildren<SpriteRenderer>() != null)
            {
                swapeImage.sprite = currentPickup.prefab.GetComponentInChildren<SpriteRenderer>().sprite;
            }
            else
            {
                swapeImage.sprite = currentPickup.prefab.GetComponent<SpriteRenderer>().sprite;
            }
                currentPickup.GetComponent<SpriteRenderer>().sprite = swapeImage.sprite;
            
        }
    }

    public void UseGlual()
    {
        if (Glual > 0)
        {
            Instantiate(GlualPrefab, firePoints[0].position, Quaternion.identity);
            Glual--;
            UpdateText();
        }
        
    }

    public void UseAiRobots()
    {
        if (AiRobots > 0)
        {
            Instantiate(AiRobotsPrefab, firePoints[0].position, Quaternion.identity);
            AiRobots--;
            UpdateText();

        }
    }

    public void UseObstacles()
    {
        if (obstcles > 0)
        {
            Instantiate(obstclesPrefab, firePoints[0].position, Quaternion.identity);
            obstcles--;
            UpdateText();
        }
    }

    public void UseTreeHide()
    {
        if (TreeHide > 0)
        {
            Instantiate(TreeHidePrefab, firePoints[0].position, Quaternion.identity);
            TreeHide--;
            UpdateText();
        }
    }

    public void UseSpeedBooster()
    {
        if (SpeedBoast > 0)
        {
            StartCoroutine(SpeedBoost());
            SpeedBoast--;
            UpdateText();
        }
    }

    IEnumerator SpeedBoost()
    {
        moveSpeed = 5;
        yield return new WaitForSeconds(10f);
        moveSpeed = 3;
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
