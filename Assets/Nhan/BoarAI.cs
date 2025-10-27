using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BoarAI : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Patrol Settings")]
    public float arriveThreshold = 0.5f;
    public bool pingPong = true;

    [Header("Idle Settings")]
    public float idleMin = 4f;
    public float idleMax = 5f;
    // animation khi đến điểm
    public bool playEatOnIdle = true;
    public string eatBoolName = "Eat";

    [Header("Perception / Combat")]
    public float detectionRadius = 12f;
    public float attackRange = 2f;               // khoảng cách để dùng Attack
    public LayerMask targetMask;                 // layer của player
    public LayerMask obstacleMask;               // để raycast che khuất (tùy chọn)

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int attackDamage = 20;
    public float attackRangeSphere = 1.5f;       // OverlapSphere bán kính gây sát thương

    [Header("Charge (Ủi)")]
    [Range(0f, 1f)] public float chargeChance = 0.5f; // xác suất charge khi phát hiện ở xa
    public float chargeSpeedMultiplier = 2.2f;
    public float chargeDuration = 1.0f;
    public float chargeCooldown = 3.0f;
    public float chargeDistance = 6f;
    public int chargeDamage = 30;
    public float chargeKnockbackForce = 6f;
    public string chargeBoolName = "isCharging";

    [Header("General")]
    public float rotationSpeed = 8f;

    // components
    private NavMeshAgent agent;
    private Animator animator;

    // state
    private int currentIndex = 0;
    private int direction = 1;
    private bool isIdling = false;
    private bool isCharging = false;
    private bool isDead = false;
    private bool canAttack = true;
    private bool canCharge = true;
    private Transform currentTarget = null;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("BoarAI cần một NavMeshAgent component.");
            enabled = false;
        }
    }

    void Start()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            SetWalking(false);
            return;
        }

        GoToPoint(currentIndex);
    }

    void Update()
    {
        if (isDead) return;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        // Tìm player trong detectionRadius
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
        if (hits.Length > 0)
        {
            // chọn player gần nhất
            currentTarget = hits[0].transform;
            float best = Vector3.Distance(transform.position, currentTarget.position);
            foreach (var h in hits)
            {
                float d = Vector3.Distance(transform.position, h.transform.position);
                if (d < best)
                {
                    best = d;
                    currentTarget = h.transform;
                }
            }

            // nếu đang đang charge thì để coroutine xử lý
            if (!isCharging)
            {
                float dist = Vector3.Distance(transform.position, currentTarget.position);

                // optional: kiểm tra line of sight (không xuyên tường)
                bool hasLOS = true;
                RaycastHit hitInfo;
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                                     (currentTarget.position - transform.position).normalized,
                                     out hitInfo, detectionRadius, ~0, QueryTriggerInteraction.Ignore))
                {
                    // nếu raycast chạm trước khi tới target và layer đó nằm trong obstacleMask thì block LOS
                    if (((1 << hitInfo.collider.gameObject.layer) & obstacleMask) != 0 &&
                        hitInfo.collider.transform != currentTarget)
                    {
                        hasLOS = false;
                    }
                }

                if (!hasLOS)
                {
                    // không thấy trực tiếp -> di chuyển tới vị trí target (hoặc không, tùy ý)
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget.position);
                    SetWalking(true);
                }
                else
                {
                    // nếu ở gần -> attack
                    if (dist <= attackRange)
                    {
                        if (canAttack) StartCoroutine(DoAttack());
                    }
                    else
                    {
                        // nếu ở xa: có thể charge (dựa trên chance), hoặc di chuyển bình thường
                        if (canCharge && Random.value <= chargeChance)
                        {
                            StartCoroutine(DoChargeTowards(currentTarget.position));
                        }
                        else
                        {
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
            // patrol nếu không thấy target
            if (!isIdling && !isCharging)
            {
                if (ReachedDestination())
                {
                    StartCoroutine(IdleThenNext());
                }
            }
        }

        // Quay hướng di chuyển mượt mà khi không charge
        if (!isCharging && agent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion look = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }
    }

    // Helper: kiểm tra đã tới destination chính xác hơn
    private bool ReachedDestination()
    {
        if (agent == null) return false;
        if (agent.pathPending) return false;
        if (!agent.hasPath) return false;
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return false;
        if (agent.remainingDistance <= arriveThreshold) return true;
        return false;
    }

    private void GoToPoint(int index)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        index = Mathf.Clamp(index, 0, patrolPoints.Length - 1);
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[index].position);
        SetWalking(true);
    }

    private IEnumerator IdleThenNext()
    {
        isIdling = true;
        agent.isStopped = true;
        SetWalking(false);

        // bật animation Eat (SetBool true) nếu bật tùy chọn
        if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(eatBoolName))
        {
            animator.SetBool(eatBoolName, true);
        }

        float waitTime = Random.Range(idleMin, idleMax);
        yield return new WaitForSeconds(waitTime);

        // tắt Eat khi kết thúc idle
        if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(eatBoolName))
        {
            animator.SetBool(eatBoolName, false);
        }

        // Next point (chú ý nếu chỉ 1 point thì không change index)
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

    private IEnumerator DoAttack()
    {
        if (!canAttack) yield break;
        canAttack = false;

        agent.isStopped = true;
        SetWalking(false);

        // play attack trigger
        animator?.SetTrigger("Attack");

        // đợi timing để match animation (tùy chỉnh nếu cần)
        yield return new WaitForSeconds(0.25f);

        // hit check
        Vector3 center = transform.position + transform.forward * 0.8f + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, attackRangeSphere, targetMask);
        foreach (var h in hits)
        {
            var health = h.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
            else
            {
                Rigidbody rb = h.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce((h.transform.position - transform.position).normalized * 4f, ForceMode.Impulse);
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        agent.isStopped = false;
    }

    // Charge hướng tới vị trí aimPosition
    private IEnumerator DoChargeTowards(Vector3 aimPosition)
    {
        if (!canCharge) yield break;
        canCharge = false;
        isCharging = true;

        animator?.SetBool(chargeBoolName, true);
        agent.isStopped = false;

        float originalSpeed = agent.speed;
        agent.speed = originalSpeed * chargeSpeedMultiplier;

        // hướng charge
        Vector3 dir = (aimPosition - transform.position).normalized;
        if (dir == Vector3.zero) dir = transform.forward;

        Vector3 desiredPos = transform.position + dir * chargeDistance;
        NavMeshHit navHit;
        Vector3 finalPos = desiredPos;
        if (NavMesh.SamplePosition(desiredPos, out navHit, 2.0f, NavMesh.AllAreas))
            finalPos = navHit.position;

        agent.SetDestination(finalPos);

        float timer = 0f;
        HashSet<Collider> hitOnce = new HashSet<Collider>();

        while (timer < chargeDuration)
        {
            // OverlapSphere phía trước để detect va chạm
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1f, 1.2f, targetMask);
            foreach (var h in hits)
            {
                if (hitOnce.Contains(h)) continue;
                hitOnce.Add(h);

                var health = h.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(chargeDamage);
                }
                Rigidbody rb = h.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 kbDir = (h.transform.position - transform.position).normalized + Vector3.up * 0.2f;
                    rb.AddForce(kbDir * chargeKnockbackForce, ForceMode.Impulse);
                }
            }

            // dừng sớm nếu đến đích
            if (!agent.pathPending && agent.remainingDistance <= 0.3f) break;

            timer += Time.deltaTime;
            yield return null;
        }

        // reset
        agent.speed = originalSpeed;
        animator?.SetBool(chargeBoolName, false);
        isCharging = false;

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator?.SetTrigger("Die"); 
        gameObject.tag = "Untagged";
        agent.enabled = false;
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.enabled = false;
        
        
        // disable collider / component nếu cần
    }

    private void SetWalking(bool isWalking)
    {
        if (animator != null)
            animator.SetBool("isWalking", isWalking);
    }

    // Gizmos debuga
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
