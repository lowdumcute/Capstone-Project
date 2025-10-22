using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;
    public float StartWaitTime = 4;
    public float TimeToRotate = 2;
    public float SpeedWalk = 6;
    public float SpeedRun = 9;


    public float ViewRadius = 15;
    public float ViewAngle = 90;
    public LayerMask playerMask;
    public LayerMask ostacleMask;

    public float meshResolution = 1f;
    public int Iteration = 4;
    public Transform[] WayPoint;

    int m_currentwaypointIndex;

    public Vector3 LastedPlayerPosition = Vector3.zero;
    public Vector3 m_PlayerPosition;

    float m_waitTime;
    float m_TimeRotate;

    bool m_PlayerInRange;
    bool m_PlayerIsNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;
    private void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_waitTime = StartWaitTime;
        m_TimeRotate = TimeToRotate;
        m_currentwaypointIndex = 0;
        agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;
        agent.speed = SpeedWalk;
        agent.SetDestination(WayPoint[m_currentwaypointIndex].position); 

    }
    private void Update()
    {
        EnviromentView();

        if(!m_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }
    void Chasing()
    {
        m_PlayerIsNear = false;
        LastedPlayerPosition = Vector3.zero;
        if (!m_CaughtPlayer)
        {
            Move(SpeedRun);
            agent.SetDestination(m_PlayerPosition);
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (m_waitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                m_IsPatrol = true;
                m_PlayerIsNear = false;
                Move(SpeedWalk);
                m_TimeRotate = TimeToRotate;
                m_waitTime = StartWaitTime;
                agent.SetDestination(WayPoint[m_currentwaypointIndex].position);
            }
            else
            {
                 if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                {
                    Stop();
                    m_waitTime -= Time.deltaTime;       
                }
            }
        }
    }
    void Patroling()
    {
        if(m_PlayerIsNear)
        {
            if(m_TimeRotate <= 0)
            {
                Move(SpeedWalk);
                LookingPlayer(LastedPlayerPosition);
            }
            else
            {
                Stop();
                m_TimeRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerIsNear = false;
            LastedPlayerPosition = Vector3.zero;
            agent.SetDestination(WayPoint[m_currentwaypointIndex].position);
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                if(m_waitTime <=0)
                {
                    NextWayPoint();
                    Move(SpeedWalk);
                    m_waitTime = StartWaitTime;
                }
            }
        }

    }
    private void Move(float Speed)
    {
        agent.isStopped = false;
        agent.speed = Speed;
    }
    private void Stop( )
    {
        agent.isStopped = true;
        agent.speed = 0;
    }
    public void NextWayPoint()
    {
        m_currentwaypointIndex = (m_currentwaypointIndex + 1) % WayPoint.Length;
        agent.SetDestination(WayPoint[m_currentwaypointIndex].position);
    }
    public void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }
    void LookingPlayer(Vector3 Player)
    {
        agent.SetDestination(Player);
        if(Vector3.Distance(transform.position,Player ) <= 0.3)
        {
            if(m_waitTime <= 0)
            {
                m_PlayerIsNear = false;
                Move(SpeedWalk);
                agent.SetDestination(WayPoint[m_currentwaypointIndex].position);
                m_waitTime = StartWaitTime;
                m_TimeRotate = TimeToRotate;
            }
            else
            {
                Stop();
                m_waitTime -= Time.deltaTime;
            }
        }
    }
    void EnviromentView()
    {
        Collider[] PlayerInRange = Physics.OverlapSphere(transform.position, ViewRadius, playerMask);
        for (int i = 0; i < PlayerInRange.Length; i++)
        {
            Transform Player = PlayerInRange[i].transform;
            Vector3 DirToPlayer = (transform.position - Player.position).normalized;
            if (Vector3.Angle(transform.forward, DirToPlayer) < ViewAngle / 2)
            {
                float DistanceToPlayer = Vector3.Distance(transform.position, Player.position);
                if (Physics.Raycast(transform.position, DirToPlayer, DistanceToPlayer, ostacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                }
                else
                {
                    m_PlayerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, Player.position) > ViewRadius)
            {
                m_PlayerInRange = false;
            }
            if (m_PlayerInRange)
            {
                m_PlayerPosition = Player.transform.position;
            }
        }

    }

}
