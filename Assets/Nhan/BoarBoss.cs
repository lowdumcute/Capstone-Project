using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BoarBoss : StateMachineBehaviour
{

    Transform PlayerPos;
    Transform BossPos;
    NavMeshAgent agent;
    Animator animator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPos = GameObject.FindGameObjectWithTag("Player").transform;
        BossPos = animator.GetComponent<BossStatus>().transform;
        agent = animator.GetComponent<NavMeshAgent>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(PlayerPos.position, BossPos.position) < 5)
        {
            int random = Random.Range(0, 5);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
            animator.ResetTrigger("Attack4");
            animator.ResetTrigger("Attack5");
            animator.ResetTrigger("Attack6");
            switch (random)
            {
                case 0:
                    animator.SetTrigger("Attack");                  
                    break;
                case 1:
                    animator.SetTrigger("Attack2");                 
                    break;
                case 2:
                    animator.SetTrigger("Attack3");
                    break;
                case 3:
                    animator.SetTrigger("Attack4");
                    break;
                case 4:
                    animator.SetTrigger("Attack5");
                    break;
                case 5:
                    animator.SetTrigger("Attack6");
                    break;
            }
            return;
        }
        else if(Vector3.Distance(PlayerPos.position, BossPos.position) > 4 &&
                Vector3.Distance(PlayerPos.position, BossPos.position) < 15)
        {
            animator.SetBool("Run Forward", true);
            
            agent.SetDestination(PlayerPos.transform.position);
           
        }      
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
