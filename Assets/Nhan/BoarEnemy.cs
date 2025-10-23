using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyBoarAI : MonoBehaviour
{
    public enum State { Idle, Wander, Forage, Alert, Investigate, Chase, Attack, Charge, PostAttackPatrol, Stagger, Dead }
    private State currentState;

    [Header("References")]
    public Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Stats")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float wanderRadius = 5f;
    public float detectRange = 10f;
    public float attackRange = 2f;

    [Header("Combat")]
    public float contactDamage = 1f;
    public float attackCooldown = 2f;
    private float attackTimer;

    private Vector3 wanderTarget;
    private float stateTimer;

    private bool sawTarget = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }

        EnterState(State.Idle);
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        // Update cooldown
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) EnterState(State.Wander);
                DetectTarget();
                break;

            case State.Wander:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    EnterState(State.Idle);
                }
                DetectTarget();
                break;

            case State.Chase:
                if (target == null) return;
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist <= attackRange)
                {
                    EnterState(State.Attack);
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = runSpeed;
                    agent.SetDestination(target.position);
                    animator.SetBool("Run Forward", true);
                }
                break;

            case State.Attack:
                // chờ animation event gọi OnAttackEnd()
                break;

            case State.PostAttackPatrol:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) EnterState(State.Chase);
                break;
        }
    }

    // ---------------- STATE MACHINE ----------------
    void EnterState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                stateTimer = Random.Range(1f, 3f);
                break;

            case State.Wander:
                agent.isStopped = false;
                agent.speed = walkSpeed;
                wanderTarget = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(wanderTarget);
                animator.SetBool("isWalking", true);
                break;

            case State.Chase:
                agent.isStopped = false;
                agent.speed = runSpeed;
                animator.SetBool("isRunning", true);
                break;

            case State.Attack:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);

                if (attackTimer <= 0f)
                {
                    int atkIndex = Random.Range(1, 4); // Attack1, Attack2, Attack3
                    animator.SetTrigger("Attack" + atkIndex);
                    attackTimer = attackCooldown;
                }
                break;
            case State.PostAttackPatrol:
                stateTimer -= Time.deltaTime;

                // luôn nhìn về phía player
                if (target != null)
                {
                    Vector3 dir = (target.position - transform.position).normalized;
                    dir.y = 0; // không xoay theo trục Y
                    if (dir.sqrMagnitude > 0.01f)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                    }
                }

                // nếu tới điểm đi bộ rồi thì chọn điểm mới (để nó không đứng im)
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    wanderTarget = RandomNavSphere(transform.position, 4f);
                    agent.SetDestination(wanderTarget);
                }

                if (stateTimer <= 0f)
                {
                    EnterState(State.Chase);
                }
                break;
                ;

            case State.Stagger:
                agent.isStopped = true;
                animator.SetTrigger("Stagger");
                break;

            case State.Dead:
                agent.isStopped = true;
                animator.SetTrigger("Die");
                Destroy(gameObject, 5f);
                break;
        }
    }

    // ---------------- HELPER ----------------
    void DetectTarget()
    {
        if (target == null) return;
        if (Vector3.Distance(transform.position, target.position) <= detectRange)
        {
            sawTarget = true;
            EnterState(State.Chase);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }

    // ---------------- ANIMATION EVENTS ----------------
    public void OnAttackHit()
    {
        if (target == null) return;
        if (Vector3.Distance(transform.position, target.position) <= attackRange + 0.3f)
        {
            PlayerHealth h = target.GetComponent<PlayerHealth>();
            if (h != null) h.TakeDamage(contactDamage);
        }
    }

    public void OnAttackEnd()
    {
        EnterState(State.PostAttackPatrol);
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(float dmg)
    {
        if (currentState == State.Dead) return;

        currentHealth -= dmg;
        if (currentHealth <= 0f)
        {
            EnterState(State.Dead);
        }
        else
        {
            EnterState(State.Stagger);
        }
    }
}
