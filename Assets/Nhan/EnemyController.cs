using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    [Header("Perception")]
    public float detectRange = 15f;
    public float attackRange = 3f;

    [Header("Chase limit")]
    public bool limitChaseByHome = false;
    public float chaseLimitFromHome = 30f;

    [Header("Patrol (fixed points)")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    public float pointSampleMaxDistance = 1.5f;

    [Header("Roam / Home")]
    public float maxRoamDistance = 12f;      // nếu ra ngoài khoảng này -> return home
    public float returnHomeTolerance = 1f;   // coi như đã về home khi < tolerance

    [Header("Movement")]
    public float stoppingDistanceTolerance = 0.5f;

    // runtime
    [HideInInspector] public int patrolIndex = 0;
    private Vector3 currentPatrolPoint = Vector3.zero;
    private bool waiting = false;
    private float waitTimer = 0f;
    private Transform player;
    private Vector3 homePosition;
    private bool returningHome = false;

    void Start()
    {
        animator = animator == null ? GetComponent<Animator>() : animator;
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        homePosition = transform.position;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            patrolIndex = Mathf.Clamp(patrolIndex, 0, patrolPoints.Length - 1);
            SetCurrentPatrolPointFromArray();
            if (agent.isOnNavMesh)
                agent.SetDestination(currentPatrolPoint);
        }
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        // Update animator speed
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        float distToPlayer = float.MaxValue;
        if (player != null) distToPlayer = Vector3.Distance(player.position, transform.position);
        float distFromHome = Vector3.Distance(transform.position, homePosition);

        // Priority: Attack > Chase (if allowed) > ReturnHome(if too far) > Patrol

        // 1) ATTACK
        if (distToPlayer <= attackRange)
        {
            if (!agent.isStopped) agent.isStopped = true;

            // ✅ Quay mặt về hướng player
            if (player != null)
            {
                Vector3 dir = (player.position - transform.position);
                dir.y = 0; // giữ enemy đứng thẳng, không ngửa lên/xuống
                if (dir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
                }
            }

            // Gửi trigger tấn công
            animator.SetTrigger("Attack");
            return;
        }


        // 2) CHASE (only if within detectRange and chase limit by home passes)
        if (player != null && distToPlayer <= detectRange)
        {
            bool canChase = true;
            if (limitChaseByHome)
            {
                float playerDistFromHome = Vector3.Distance(player.position, homePosition);
                if (playerDistFromHome > chaseLimitFromHome) canChase = false;
            }

            if (canChase)
            {
                // If currently returning home, cancel it
                returningHome = false;
                waiting = false;
                DoChase();
                return;
            }
            else
            {
                // Player detected but too far from home to chase -> fall through to returnHome/patrol
            }
        }

        // 3) RETURN HOME if roamed too far
        if (distFromHome > maxRoamDistance)
        {
            // start returning home
            returningHome = true;
            waiting = false;

            if (agent.isStopped) agent.isStopped = false;
            agent.SetDestination(homePosition);

            // arrived home?
            if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, returnHomeTolerance))
            {
                returningHome = false;
                // when arrive, advance patrol index and set next patrol point
                AdvancePatrolIndex();
                SetCurrentPatrolPointFromArray();
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(currentPatrolPoint);
                }
            }
            return;
        }

        // 4) Patrol / Idle (no player chase and not returningHome)
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            if (!agent.isStopped) agent.ResetPath();
            return;
        }

        // if waiting at point -> countdown
        if (waiting)
        {
            if (!agent.isStopped) agent.isStopped = true;
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                AdvancePatrolIndex();
                SetCurrentPatrolPointFromArray();
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(currentPatrolPoint);
                }
                else
                {
                    agent.SetDestination(currentPatrolPoint);
                    agent.isStopped = false;
                }
            }
            return;
        }

        // check arrival to patrol point using agent
        if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, stoppingDistanceTolerance))
        {
            StartWaitingAtPoint();
            return;
        }

        // ensure destination set
        if (agent.destination != currentPatrolPoint)
        {
            agent.SetDestination(currentPatrolPoint);
        }
    }

    void DoChase()
    {
        if (agent.isStopped) agent.isStopped = false;
        if (player != null)
            agent.SetDestination(player.position);
    }

    void StartWaitingAtPoint()
    {
        waiting = true;
        waitTimer = patrolWaitTime;
        if (!agent.isStopped) agent.isStopped = true;
    }

    void AdvancePatrolIndex()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    void SetCurrentPatrolPointFromArray()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            currentPatrolPoint = transform.position;
            return;
        }

        Transform t = patrolPoints[patrolIndex];
        if (t == null)
        {
            currentPatrolPoint = transform.position;
            return;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(t.position, out hit, pointSampleMaxDistance, NavMesh.AllAreas))
            currentPatrolPoint = hit.position;
        else
            currentPatrolPoint = t.position;
    }

    // Public helper to force restart patrol
    public void ResetToHome()
    {
        patrolIndex = 0;
        SetCurrentPatrolPointFromArray();
        agent.Warp(homePosition);
        agent.ResetPath();
        waiting = false;
        returningHome = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxRoamDistance);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var p in patrolPoints)
            {
                if (p != null) Gizmos.DrawSphere(p.position, 0.15f);
            }
        }
    }
}
