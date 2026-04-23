using UnityEngine;

public class EnemyChaseSimple : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(player);
        }
    }
}
