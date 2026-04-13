using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private float distanceThreshold = 0.15f;
    [SerializeField] private LayerMask groundMask = ~0;

    public bool isGrounded = true;
    public event System.Action Grounded;

    private const float OriginOffset = 0.01f;

    private Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    private float RaycastDistance => distanceThreshold + OriginOffset;

    private void LateUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(RaycastOrigin, Vector3.down, RaycastDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (isGrounded && !wasGrounded)
            Grounded?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(
            RaycastOrigin,
            RaycastOrigin + Vector3.down * RaycastDistance,
            isGrounded ? Color.white : Color.red
        );
    }
}