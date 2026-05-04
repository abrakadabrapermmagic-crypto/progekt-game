using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAnimatorController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // сюда перетащить игрока

    private NavMeshAgent agent;
    private EnemyAnimatorController animController;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponent<EnemyAnimatorController>();
    }

    private void Update()
    {
        if (!target || agent == null) return;

        // Двигаемся к игроку
        agent.destination = target.position;
    }

    public void StopForAttack()
    {
        if (agent != null)
            agent.isStopped = true;
    }

    public void ResumeAfterAttack()
    {
        if (agent != null)
            agent.isStopped = false;
    }

    public void OnDeath()
    {
        if (agent != null)
            agent.isStopped = true;

        if (animController != null)
            animController.PlayDeath();
    }
}
