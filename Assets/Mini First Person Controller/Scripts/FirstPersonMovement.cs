using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5f;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;

    private Rigidbody rb;
    private Vector3 moveInput;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        IsRunning = canRun && Input.GetKey(runningKey);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveInput = (transform.right * x + transform.forward * z).normalized;
    }

    void FixedUpdate()
    {
        float targetSpeed = IsRunning ? runSpeed : speed;

        if (speedOverrides.Count > 0)
        {
            targetSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = moveInput * targetSpeed;

        rb.velocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
    }
}