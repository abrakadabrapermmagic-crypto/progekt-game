using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAttractor : MonoBehaviour
{
    public float force = 10f; // Сила притяжения

    // Объект должен находиться в триггере-сфере (установить Is Trigger = true)
    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        Vector3 direction = (transform.position - other.transform.position).normalized;
        rb.AddForce(direction * force);

    }
}