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

    [Header("Firing Assests")]
    public GameObject Bullet;
    public Transform FirePoint;
    public GameObject FireFlams;
    public bool isFire=false;
    public bool isReloading=false;
    public float timeout = 1f;
    public float damage = 10f;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip ride;
    public AudioClip fire;


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
                    }
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(fire);
                       audioSource.UnPause();
                        
                    }
                    StartCoroutine(ShootAndRealoding(timeout));
                }
                
            }
        }
    }

    IEnumerator ShootAndRealoding(float timeout)
    {
        isFire = false;
        isReloading=true;
        FireFlams.SetActive(true);
        GameObject bullet = Instantiate(
     Bullet,
     FirePoint.position,
     Quaternion.Euler(0, 0, barrelPoint.eulerAngles.z + 180f)
 );
        bullet.GetComponent<Bullet>().damage=damage;

        yield return new WaitForSeconds(0.1f);
        FireFlams.SetActive(false);
        yield return new WaitForSeconds(timeout);
        isReloading= false;
    }
}
