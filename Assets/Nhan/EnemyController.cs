using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
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

    [Header("General")]
    public float rotationSpeed = 8f;

    // components
    private NavMeshAgent agent;
    private Animator animator;

    // state
    private int currentIndex = 0;
    private int direction = 1;
    private bool isIdling = false;
    private bool isDead = false;
    private bool canAttack = true;
    private Transform currentTarget = null;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("EnemyController cần một NavMeshAgent component.");
            enabled = false;
        }
    }

    void Start()
    {
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

        // Luôn cập nhật speed cho animator
        float currentSpeed = agent.velocity.magnitude;
        animator?.SetFloat("Speed", currentSpeed);

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

            // nếu đang idle thì ngắt idle để chase ngay
            if (isIdling)
            {
                isIdling = false;
                agent.isStopped = false;
                if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(eatBoolName))
                    animator.SetBool(eatBoolName, false);
            }

            float dist = Vector3.Distance(transform.position, currentTarget.position);

            // optional: kiểm tra line of sight (không xuyên tường)
            bool hasLOS = true;
            RaycastHit hitInfo;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 dir = (currentTarget.position - origin).normalized;
            if (Physics.Raycast(origin, dir, out hitInfo, detectionRadius))
            {
                if (hitInfo.collider != null && hitInfo.transform != currentTarget)
                {
                    if (((1 << hitInfo.collider.gameObject.layer) & obstacleMask) != 0)
                        hasLOS = false;
                }
            }

            if (!hasLOS)
            {
                // không thấy trực tiếp -> vẫn chase vị trí target
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
                
            }
            else
            {
                // Nếu ở trong bán kính attackRangeSphere -> attack
                if (dist <= attackRangeSphere)
                {
                    if (canAttack) StartCoroutine(DoAttack());
                }
                else
                {
                    // Nếu xa -> chase bình thường (không charge)
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget.position);
    
                }
            }
        }
        else
        {
            currentTarget = null;
            // patrol nếu không thấy target
            if (!isIdling)
            {
                if (ReachedDestination())
                {
                    StartCoroutine(IdleThenNext());
                }
            }
        }

        // Quay hướng di chuyển mượt mà
        if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
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

    }

    private IEnumerator IdleThenNext()
    {
        isIdling = true;
        agent.isStopped = true;


        // bật animation Eat (SetBool true) nếu bật tùy chọn
        if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(eatBoolName))
        {
            animator.SetBool(eatBoolName, true);
        }

        float waitTime = Random.Range(idleMin, idleMax);

        // Trong khi idle, nếu phát hiện player thì thoát sớm
        float elapsed = 0f;
        while (elapsed < waitTime)
        {
            Collider[] h = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
            if (h.Length > 0)
            {
                // phát hiện player -> ngắt idle
                isIdling = false;
                if (playEatOnIdle && animator != null && !string.IsNullOrEmpty(eatBoolName))
                    animator.SetBool(eatBoolName, false);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

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

        // stop di chuyển khi attack
        agent.isStopped = true;

        // play attack trigger
        animator?.SetTrigger("Attack");

        // đặt Speed = 0 khi attack (đảm bảo animator nhận biết)
        animator?.SetFloat("Speed", 0f);

        // đợi timing để match animation (tùy chỉnh nếu cần)
        yield return new WaitForSeconds(0.25f);

        // hit check (dùng attackRangeSphere)
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


    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * 0.8f + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(center, attackRangeSphere);
    }
}
