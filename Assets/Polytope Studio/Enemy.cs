using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float aggroRange = 12f;
    [SerializeField] private float resetDistance = 15f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 4f;
    [SerializeField] private float patrolInterval = 5f; // mỗi 5s sẽ đi tuần tra
    [SerializeField] private bool startPatrolImmediately = false; // true để patrol ngay sau khi spawn
    [SerializeField] private int patrolSampleAttempts = 10; // số lần thử tìm điểm trên NavMesh

    [Header("Refs")]
    public EnemySpawner spawner;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private Collider col;
    [SerializeField] private Collider AttackColider;

    private Vector3 beginPosition;
    private Vector3 currentPatrolTarget;
    private float patrolTimer;
    private bool hasPatrolTarget;

    private float attackTimer;
    private float currentHealth;
    private bool isDead;

    private bool inAction = false;

    private enum State { Idle, Patrol, Chase, Attack, Return }
    private State state = State.Idle;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        beginPosition = transform.position;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        patrolTimer = startPatrolImmediately ? patrolInterval : 0f;

        if (agent == null)
        {
            Debug.LogError($"{name}: Không tìm thấy NavMeshAgent!");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: NavMeshAgent không trên NavMesh. Hãy bake NavMesh hoặc spawn enemy trên NavMesh.");
        }

        agent.updatePosition = true;
        agent.updateRotation = true;
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;
        if (agent == null) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        float distanceFromHome = Vector3.Distance(transform.position, beginPosition);
        attackTimer += Time.deltaTime;

        float speedRatio = (agent.speed > 0.0001f) ? agent.velocity.magnitude / agent.speed : 0f;
        animator.SetFloat("Speed", speedRatio);

        // LƯU Ý: KHÔNG interrupt Return bởi player gần.
        // Nếu enemy đang Return thì nó sẽ ưu tiên hoàn thành Return đến beginPosition.
        // Khi đã ở Home thì các trạng thái bình thường mới được chấp nhận.

        // Nếu enemy đi quá xa home -> bắt đầu Return (ưu tiên)
        if (distanceFromHome > resetDistance)
        {
            // khi đặt Return, huỷ mọi mục tiêu patrol
            state = State.Return;
            hasPatrolTarget = false;
        }

        // nếu player tiến vào aggroRange và đang ở Idle/Patrol -> chuyển sang Chase
        if ((state == State.Idle || state == State.Patrol) && distanceToPlayer < aggroRange)
        {
            state = State.Chase;
            hasPatrolTarget = false;
        }

        switch (state)
        {
            case State.Idle:
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolInterval && !inAction)
                {
                    TryStartPatrol();
                }
                break;

            case State.Patrol:
                if (!hasPatrolTarget)
                {
                    state = State.Idle;
                    patrolTimer = 0f;
                    break;
                }

                if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.1f))
                {
                    hasPatrolTarget = false;
                    patrolTimer = 0f;
                    state = State.Idle;
                    agent.ResetPath();
                }
                break;

            case State.Chase:
                // Chase chỉ xảy ra nếu không đang Return — do chúng ta không cho interrupt Return
                if (distanceToPlayer <= attackRange)
                {
                    state = State.Attack;
                }
                else
                {
                    if (!inAction && agent.isOnNavMesh)
                        agent.SetDestination(player.position);
                }
                break;

            case State.Attack:
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                if (!inAction)
                {
                    if (attackTimer >= attackCooldown)
                    {
                        attackTimer = 0f;
                        agent.isStopped = true;
                        inAction = true;
                        animator.SetTrigger("Attack");
                    }
                }

                if (!inAction && Vector3.Distance(player.position, transform.position) > attackRange)
                {
                    state = State.Chase;
                }
                break;

            case State.Return:
                // Luôn tiếp tục về home; KHÔNG đổi sang Chase ngay cả khi player gần
                if (!inAction && agent.isOnNavMesh)
                    agent.SetDestination(beginPosition);

                if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.1f))
                {
                    // Đã về Home -> trở lại Idle (và lúc này nếu player đang trong aggroRange sẽ bị bắt lên Chase bởi check phía trên)
                    state = State.Idle;
                    agent.ResetPath();
                    hasPatrolTarget = false;
                    patrolTimer = 0f;
                }
                break;
        }
    }

    private void TryStartPatrol()
    {
        if (agent == null || !agent.isOnNavMesh || patrolRadius <= 0f)
        {
            patrolTimer = 0f;
            return;
        }

        for (int i = 0; i < Mathf.Max(1, patrolSampleAttempts); i++)
        {
            Vector3 randomPoint = beginPosition + Random.insideUnitSphere * patrolRadius;
            randomPoint.y = beginPosition.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                currentPatrolTarget = hit.position;
                hasPatrolTarget = true;
                patrolTimer = 0f;
                state = State.Patrol;
                agent.SetDestination(currentPatrolTarget);
                Debug.DrawLine(transform.position, currentPatrolTarget, Color.green, 2f);
                Debug.Log($"{name}: Patrol started -> target {currentPatrolTarget} (attempt {i + 1})");
                return;
            }
        }

        Debug.LogWarning($"{name}: Không tìm được điểm patrol sau {patrolSampleAttempts} lần. Kiểm tra NavMesh hoặc tăng patrolRadius.");
        patrolTimer = 0f;
    }

    // Animation events
    public void OnAttackStart()
    {
        inAction = true;
        if (AttackColider != null) AttackColider.enabled = true;
        if (agent != null) agent.isStopped = true;
    }

    public void OnAttackEnd()
    {
        // Khi attack kết thúc, nếu đang Return thì giữ Return (ưu tiên hoàn thành)
        inAction = false;
        if (AttackColider != null) AttackColider.enabled = false;
        if (agent != null) agent.isStopped = false;

        // Nếu hiện tại state là Return thì không đổi state ở đây — Return vẫn giữ ưu tiên
        if (state == State.Return)
        {
            // nothing more, Return sẽ tiếp tục trong Update
            return;
        }

        // Ngược lại, quyết định theo khoảng cách hiện tại
        float distanceToPlayer = (player != null) ? Vector3.Distance(player.position, transform.position) : float.MaxValue;
        if (distanceToPlayer <= attackRange) state = State.Attack;
        else if (distanceToPlayer <= aggroRange) state = State.Chase;
        else state = State.Return;
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        animator.SetTrigger("Damage");

        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Die");
            if (agent != null) agent.isStopped = true;
            if (col != null) col.enabled = false;
            Invoke(nameof(DisableAfterDie), 1.2f);
        }
    }

    private void DisableAfterDie()
    {
        if (spawner != null) spawner.NotifyEnemyDeath(this);
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        isDead = false;
        state = State.Idle;
        attackTimer = 0f;
        inAction = false;
        patrolTimer = 0f;
        hasPatrolTarget = false;

        if (agent != null)
        {
            agent.enabled = true;
            if (agent.isOnNavMesh) agent.Warp(beginPosition);
            else transform.position = beginPosition;
            agent.ResetPath();
            agent.isStopped = false;
        }
        else transform.position = beginPosition;

        if (col != null) col.enabled = true;
        animator.Play("Idle", -1, 0f);
        gameObject.SetActive(true);
    }

    [ContextMenu("Force Patrol")]
    public void ForcePatrol()
    {
        TryStartPatrol();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(beginPosition == Vector3.zero ? transform.position : beginPosition, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(beginPosition == Vector3.zero ? transform.position : beginPosition, resetDistance);

        if (hasPatrolTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentPatrolTarget + Vector3.up * 0.1f, 0.15f);
            Gizmos.DrawLine(transform.position, currentPatrolTarget);
        }
    }
}
