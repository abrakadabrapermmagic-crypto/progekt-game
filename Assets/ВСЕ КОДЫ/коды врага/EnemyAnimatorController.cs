using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimatorController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int AttackHash = Animator.StringToHash("Attack");
    private readonly int DeathHash = Animator.StringToHash("Death");

    [SerializeField] private float speedDampTime = 0.1f; // Плавность перехода анимации

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Используем дельту времени для плавности, чтобы персонаж не "скользил"
        float speed = agent.velocity.magnitude / agent.speed;
        anim.SetFloat(SpeedHash, speed, speedDampTime, Time.deltaTime);
    }

    public void PlayAttack()
    {
        anim.SetTrigger(AttackHash);
    }

    public void PlayDeath()
    {
        anim.SetTrigger(DeathHash);

        // Полностью отключаем агента, чтобы он не мешал навигации
        agent.isStopped = true;
        agent.enabled = false;
    }
}