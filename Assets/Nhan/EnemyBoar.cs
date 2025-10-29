using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyBoar : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float arriveThreshold = 0.5f;
    public bool pingPong = true;

    [Header("Speeds")]
    public float patrolSpeed = 2f;      // tốc độ khi đi patrol
    public float chaseSpeed = 4f;       // tốc độ khi chase player

    [Header("Idle")]
    public float idleMin = 3f;
    public float idleMax = 5f;
    public bool playEatOnIdle = true;
    public string BoolName = "Eat";

    [Header("Perception / Combat")]
    public float detectionRadius = 12f;
    public float attackRange = 2.8f;          // khoảng cách để attack (logic)
    public float attackRangeSphere = 1.5f;    // OverlapSphere bán kính gây sát thương
    public LayerMask targetMask;
    public LayerMask obstacleMask;            // để kiểm tra line-of-sight

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int attackDamage = 20;

    [Header("Charge (Ủi)")]
    [Range(0f, 1f)] public float chargeChance = 0.6f; // xác suất charge khi decide
    public bool allowChargeWhilePatrol = true;      // nếu true, phát hiện player lúc patrol vẫn có thể charge
    public float chargeSpeedMultiplier = 2.5f;     // nhân với chaseSpeed
    public float chargeDuration = 1.0f;
    public float chargeCooldown = 3.0f;
    public float chargeDistance = 6f;
    public int chargeDamage = 30;
    public float chargeKnockbackForce = 6f;
    public string chargeBoolName = "isCharging";

    [Header("Animator / Rotation")]
    public float rotationSpeed = 8f;

    // components
    NavMeshAgent agent;
    Animator animator;

    // state
    int currentIndex = 0;
    int direction = 1;
    bool isIdling = false;
    bool isDead = false;
    bool canAttack = true;
    bool canCharge = true;
    bool isCharging = false;
    Transform currentTarget = null;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent == null)
        {
            Debug.LogError("EnemyBoarWithCharge cần NavMeshAgent.");
            enabled = false;
        }
    }

    void Start()
    {
        // set default speed cho patrol
        agent.speed = patrolSpeed;

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }
        GoToPoint(currentIndex);
    }

    void Update()
    {
        if (isDead) return;

        // cập nhật Speed param (dùng speed thực tế của agent)
        animator?.SetFloat("Speed", agent.velocity.magnitude);

        // detect player
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
        if (hits.Length > 0)
        {
            // chọn target gần nhất
            Transform best = null;
            float bestDist = float.MaxValue;
            foreach (var c in hits)
            {
                if (c == null) continue;
                float d = Vector3.Distance(transform.position, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c.transform;
                }
            }
            currentTarget = best;

            // if was idling, stop idle
            if (isIdling)
            {
                isIdling = false;
                agent.isStopped = false;
                if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(BoolName))
                    animator.SetBool(BoolName, false);
            }

            if (!isCharging)
            {
                float dist = Vector3.Distance(transform.position, currentTarget.position);

                // line of sight check (optional)
                bool hasLOS = true;
                RaycastHit info;
                Vector3 origin = transform.position + Vector3.up * 0.5f;
                Vector3 dir = (currentTarget.position - origin).normalized;
                if (Physics.Raycast(origin, dir, out info, detectionRadius))
                {
                    if (info.collider != null && info.transform != currentTarget)
                    {
                        if (((1 << info.collider.gameObject.layer) & obstacleMask) != 0)
                            hasLOS = false;
                    }
                }

                // decide to charge or chase or attack
                // - nếu đang patrol và không cho charge while patrol -> chỉ chase
                // - nếu trong attackRange -> attack
                // - nếu ngoài attackRange: có thể charge (dựa trên chance) hoặc chase (với chaseSpeed)
                if (!hasLOS)
                {
                    // không thấy trực tiếp -> chase vị trí mục tiêu
                    SetAgentSpeed(chaseSpeed);
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget.position);
                }
                else
                {
                    // attack nếu đủ gần
                    if (dist <= attackRange)
                    {
                        if (canAttack) StartCoroutine(DoAttack());
                    }
                    else
                    {
                        // có thể charge khi:
                        // - allowChargeWhilePatrol true OR đã ở trạng thái chase (gọi chase ngay khi phát hiện)
                        // sử dụng chargeChance để quyết định
                        bool canConsiderCharge = allowChargeWhilePatrol || agent.speed == chaseSpeed;
                        if (canCharge && canConsiderCharge && Random.value <= chargeChance)
                        {
                            // bắt đầu charge về hướng player
                            StartCoroutine(DoChargeTowards(currentTarget.position));
                        }
                        else
                        {
                            // chase bình thường
                            SetAgentSpeed(chaseSpeed);
                            agent.isStopped = false;
                            agent.SetDestination(currentTarget.position);
                        }
                    }
                }
            }
        }
        else
        {
            currentTarget = null;
            // patrol
            if (!isIdling && !isCharging)
            {
                if (ReachedDestination())
                {
                    StartCoroutine(IdleThenNext());
                }
            }
        }

        // rotate smoothly toward movement direction when moving and not charging
        if (!isCharging && agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }
    }

    // set agent speed and optionally update animator Speed if needed
    void SetAgentSpeed(float speed)
    {
        if (agent != null) agent.speed = speed;
    }

    // patrol helpers
    private void GoToPoint(int index)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        index = Mathf.Clamp(index, 0, patrolPoints.Length - 1);
        SetAgentSpeed(patrolSpeed);
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[index].position);
    }

    private bool ReachedDestination()
    {
        if (agent == null) return false;
        if (agent.pathPending) return false;
        if (!agent.hasPath) return false;
        if (agent.remainingDistance <= arriveThreshold) return true;
        return false;
    }

    private IEnumerator IdleThenNext()
    {
        isIdling = true;
        agent.isStopped = true;

        if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(BoolName))
            animator.SetBool(BoolName, true);

        float waitTime = Random.Range(idleMin, idleMax);
        float elapsed = 0f;
        while (elapsed < waitTime)
        {
            // nếu phát hiện player trong lúc idle -> ngắt sớm
            Collider[] h = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
            if (h.Length > 0)
            {
                isIdling = false;
                if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(BoolName))
                    animator.SetBool(BoolName, false);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(BoolName))
            animator.SetBool(BoolName, false);

        // advance patrol index
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            if (pingPong)
            {
                if (currentIndex == patrolPoints.Length - 1) direction = -1;
                else if (currentIndex == 0) direction = 1;
                currentIndex += direction;
            }
            else
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
            }
        }

        GoToPoint(currentIndex);
        isIdling = false;
    }

    // Attack coroutine
    private IEnumerator DoAttack()
    {
        if (!canAttack) yield break;
        canAttack = false;

        agent.isStopped = true;
        SetAgentSpeed(0f); // ensure speed 0 during attack (optional)
        animator?.SetFloat("Speed", 0f);

        animator?.SetTrigger("Attack");

        // wait for attack timing (tweak if your animation needs)
        yield return new WaitForSeconds(0.25f);

        // hit check
        Vector3 center = transform.position + transform.forward * 0.8f + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, attackRangeSphere, targetMask);
        foreach (var h in hits)
        {
            if (h == null) continue;
            var health = h.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(attackDamage);
            else
            {
                Rigidbody rb = h.GetComponent<Rigidbody>();
                if (rb != null) rb.AddForce((h.transform.position - transform.position).normalized * 4f, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        // restore chase speed if target exists, else patrolSpeed
        if (currentTarget != null) SetAgentSpeed(chaseSpeed);
        else SetAgentSpeed(patrolSpeed);
        agent.isStopped = false;
    }

    // Charge coroutine
    private IEnumerator DoChargeTowards(Vector3 aimPosition)
    {
        if (!canCharge) yield break;
        canCharge = false;
        isCharging = true;

        animator?.SetBool(chargeBoolName, true);

        // increase speed for charge (base on chaseSpeed)
        float originalSpeed = agent.speed;
        float chargeSpeed = chaseSpeed * chargeSpeedMultiplier;
        SetAgentSpeed(chargeSpeed);
        agent.isStopped = false;

        // compute final pos on NavMesh
        Vector3 dir = (aimPosition - transform.position).normalized;
        if (dir == Vector3.zero) dir = transform.forward;
        Vector3 desiredPos = transform.position + dir * chargeDistance;
        NavMeshHit navHit;
        Vector3 finalPos = desiredPos;
        if (NavMesh.SamplePosition(desiredPos, out navHit, 2f, NavMesh.AllAreas))
            finalPos = navHit.position;
        agent.SetDestination(finalPos);

        float timer = 0f;
        HashSet<Collider> hitOnce = new HashSet<Collider>();

        while (timer < chargeDuration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1f, 1.2f, targetMask);
            foreach (var h in hits)
            {
                if (h == null || hitOnce.Contains(h)) continue;
                hitOnce.Add(h);

                var health = h.GetComponent<PlayerHealth>();
                if (health != null) health.TakeDamage(chargeDamage);
                Rigidbody rb = h.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 kb = (h.transform.position - transform.position).normalized + Vector3.up * 0.2f;
                    rb.AddForce(kb * chargeKnockbackForce, ForceMode.Impulse);
                }
            }

            // stop early if reached nav destination
            if (!agent.pathPending && agent.remainingDistance <= 0.3f) break;

            timer += Time.deltaTime;
            yield return null;
        }

        // reset
        SetAgentSpeed(originalSpeed);
        animator?.SetBool(chargeBoolName, false);
        isCharging = false;

        // cooldown
        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    // Debug gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * 0.8f + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(center, attackRangeSphere);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * chargeDistance);
    }
}
