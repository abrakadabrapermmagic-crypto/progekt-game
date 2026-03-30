using UnityEngine;

public class Medkit : MonoBehaviour
{
    [Header("Лечение")]
    public int healAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();  // ← ИСПРАВЛЕНО
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
