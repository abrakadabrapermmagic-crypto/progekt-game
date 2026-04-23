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
    private readonly int AttackHashWithSpace = Animator.StringToHash("Attack ");
    private readonly int DeathHash = Animator.StringToHash("Death");

    [SerializeField] private float speedDampTime = 0.1f; // Плавность перехода анимации
    [SerializeField] private string fallbackAttackStateName = "Attack ";

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
        // Защита от рассинхрона параметров в Animator (Attack / "Attack ").
        anim.ResetTrigger(AttackHash);
        anim.ResetTrigger(AttackHashWithSpace);
        anim.SetTrigger(AttackHash);
        anim.SetTrigger(AttackHashWithSpace);

        // На случай отсутствия переходов по trigger принудительно включаем стейт атаки.
        if (!string.IsNullOrWhiteSpace(fallbackAttackStateName))
            anim.CrossFadeInFixedTime(fallbackAttackStateName, 0.1f);
    }

    public void PlayDeath()
    {
        anim.SetTrigger(DeathHash);

        // Полностью отключаем агента, чтобы он не мешал навигации
        agent.isStopped = true;
        agent.enabled = false;
    }
}
