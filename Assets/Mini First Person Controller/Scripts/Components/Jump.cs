using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpStrength = 5f;
    [SerializeField] private GroundCheck groundCheck;

    private Rigidbody rb;
    private bool jumpRequested;

    public event System.Action Jumped;

    private void Reset()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!groundCheck)
            groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;
    }

    private void FixedUpdate()
    {
        if (!jumpRequested)
            return;

        if (groundCheck != null && !groundCheck.isGrounded)
            return;

        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        Jumped?.Invoke();
        jumpRequested = false;
    }
}