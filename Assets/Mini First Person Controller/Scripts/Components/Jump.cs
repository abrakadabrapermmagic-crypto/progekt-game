using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rb;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    [SerializeField, Tooltip("Сбрасывать вертикальную скорость перед прыжком для стабильной высоты.")]
    bool resetVerticalVelocity = true;

    [SerializeField, Tooltip("Небольшой запас времени после схода с поверхности, когда прыжок еще разрешен.")]
    float coyoteTime = 0.1f;

    [SerializeField, Tooltip("Fallback проверки земли, если GroundCheck не назначен.")]
    float fallbackGroundCheckDistance = 0.2f;

    float lastGroundedTime;

    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        // Get rigidbody.
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb == null) return;

        if (IsGrounded())
            lastGroundedTime = Time.time;

        bool jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);
        bool canJump = (Time.time - lastGroundedTime) <= coyoteTime;

        // Jump when jump input is pressed and we are grounded (with coyote-time).
        if (jumpPressed && canJump)
        {
            if (resetVerticalVelocity)
            {
                var velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
            }

            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            Jumped?.Invoke();

            // Чтобы нельзя было сделать 2 прыжка в одном окне coyote-time.
            lastGroundedTime = -999f;
        }
    }

    bool IsGrounded()
    {
        if (groundCheck != null)
            return groundCheck.isGrounded;

        return Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, fallbackGroundCheckDistance + 0.05f);
    }
}
