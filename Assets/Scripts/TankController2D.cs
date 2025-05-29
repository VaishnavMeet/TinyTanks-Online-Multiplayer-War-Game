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

    void Start()
    {
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

            // Rotate tank body to face movement direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg ;
            angle += 90;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            tankBody.rotation = Quaternion.RotateTowards(tankBody.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleBarrelRotation()
    {
        Vector2 aimInput = new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);

        if (aimInput.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;

            // Correct angle offset (if your barrel sprite points "up" by default)
            angle += 90f;

            barrelPoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
