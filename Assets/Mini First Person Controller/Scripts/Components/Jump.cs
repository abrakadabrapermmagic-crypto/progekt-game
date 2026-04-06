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

        // Jump when the Jump button is pressed and we are on the ground.
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            if (resetVerticalVelocity)
            {
                var velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
            }

            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            Jumped?.Invoke();
        }
    }
}
