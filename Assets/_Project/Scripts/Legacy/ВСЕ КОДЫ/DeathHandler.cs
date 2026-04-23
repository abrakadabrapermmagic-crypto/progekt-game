using UnityEngine;
using UnityEngine.AI;

public class DeathHandler : MonoBehaviour
{
    public float destroyDelay = 2f;   // ? длине анимации смерти

    Animator anim;
    readonly int DeathHash = Animator.StringToHash("Death");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void HandleDeath()
    {
        anim.SetTrigger(DeathHash);          // включаем анимацию смерти[web:121][web:142]
        StartCoroutine(WaitAndDestroy());
        gameObject.GetComponent<NavMeshAgent>().speed=0;
    }

    System.Collections.IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
