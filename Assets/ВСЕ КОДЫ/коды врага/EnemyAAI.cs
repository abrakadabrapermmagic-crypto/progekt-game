using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform target;
    public float updatePathInterval = 0.2f;

    private NavMeshAgent _agent;
    private Animator _anim;
    private bool _isDead = false;
    private float _nextPathUpdateTime = 0f;

    private readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private readonly int DeathHash = Animator.StringToHash("Death");

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (target != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (_isDead || target == null || !_agent.enabled) return;

        // Обновляем путь к игроку через интервал
        if (Time.time >= _nextPathUpdateTime && _agent.isOnNavMesh && !_agent.isStopped)
        {
            _agent.SetDestination(target.position);
            _nextPathUpdateTime = Time.time + updatePathInterval;
        }

        // Анимация бега
        float speed = _agent.velocity.magnitude;
        bool moving = speed > 0.1f && !_agent.isStopped;
        _anim.SetBool(IsRunningHash, moving);
    }

    // Если нужно остановить врага для каких-то внешних событий
    public void StopMovement()
    {
        if (_isDead) return;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _anim.SetBool(IsRunningHash, false);
    }

    public void ResumeMovement()
    {
        if (_isDead) return;
        _agent.isStopped = false;
    }

    public void HandleDeath()
    {
        if (_isDead) return;
        _isDead = true;

        _agent.isStopped = true;

        if (_agent.isOnNavMesh)
            _agent.enabled = false;

        _anim.SetBool(IsRunningHash, false);
        _anim.SetTrigger(DeathHash);
    }
}