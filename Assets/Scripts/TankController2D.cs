using System.Collections;
using UnityEngine;

public class TankController2D : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;

    public Joystick moveJoystick; // Right joystick
    public Joystick aimJoystick;  // Left joystick

    public Transform tankBody;     // Reference to TankBody
    public Transform barrelPoint;  // Pivot for barrel
    public Transform barrel;       // Actual barrel sprite
    
    private Rigidbody2D rb;

    [Header("Feactures")]
    public float maxHealth = 200f;
    public float health = 200f;

    [Header("Firing Assests")]
    public GameObject Bullet;
    public Transform FirePoint;
    public GameObject FireFlams;
    public bool isFire=false;
    public bool isReloading=false;
    public float timeout = 1f;
    bool isFiveShot=false;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip ride;
    public AudioClip fire;
    bool wasPaused=false;


    void Start()
    {
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
            barrelPoint.rotation = Quaternion.Euler(0, 0, angle);

            // Fire if joystick is pushed to its limit (or very close)
            if (magnitude >= 0.95f) // adjust threshold if needed
            {
                if (!isFire && !isReloading)
                {
                    isFire= true;
                    if (audioSource.isPlaying)
                    {
                        audioSource.Pause() ;
                        wasPaused= true ;
                    }
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(fire);
                        if (wasPaused)
                        {
                            audioSource.UnPause();
                            wasPaused= false ;
                        }
                        
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
                yield return new WaitForSeconds(0.1f); // Optional delay between shots
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
        FireFlams.SetActive(true);
        GameObject bullet = Instantiate(
     Bullet,
     FirePoint.position,
     Quaternion.Euler(0, 0, barrelPoint.eulerAngles.z + 180f));
        yield return new WaitForSeconds(0.1f);
        FireFlams.SetActive(false);
    }
}
