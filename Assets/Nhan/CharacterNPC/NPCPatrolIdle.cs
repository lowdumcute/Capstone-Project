using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCPatrolIdle : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Patrol Settings")]
    public float arriveThreshold = 0.5f;
    public bool pingPong = true;

    [Header("Idle Settings")]
    public float idleMin = 4f;
    public float idleMax = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    private int currentIndex = 0;
    private int direction = 1;
    private bool isIdling = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NPCPatrolIdle cần một NavMeshAgent component.");
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
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (isIdling) return;

        // Kiểm tra đã đến point hay chưa
        if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
        {
            StartCoroutine(IdleThenNext());
        }
    }

    private void GoToPoint(int index)
    {
        if (index < 0 || index >= patrolPoints.Length) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[index].position);
        SetWalking(true);
    }

    private IEnumerator IdleThenNext()
    {
        isIdling = true;
        agent.isStopped = true;
        SetWalking(false);

        float waitTime = Random.Range(idleMin, idleMax);
        yield return new WaitForSeconds(waitTime);

        // Tính point tiếp theo
        if (pingPong)
        {
            if (currentIndex == patrolPoints.Length - 1)
                direction = -1;
            else if (currentIndex == 0)
                direction = 1;

            currentIndex += direction;
        }
        else
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
        }

        GoToPoint(currentIndex);
        isIdling = false;
    }

    private void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }
}
