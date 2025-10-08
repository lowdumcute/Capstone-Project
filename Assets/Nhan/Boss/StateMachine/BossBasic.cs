using UnityEngine;
using UnityEngine.AI;

public class BossBasic : StateMachineBehaviour
{
    Transform PlayerPos;
    Transform BossPos;
    NavMeshAgent agent;
    Animator animator;

    int FinalskillPhase1count = 3;
    int FinalskillPhase2count = 3;
    int Phase1count;
    int Phase2count;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPos = GameObject.FindGameObjectWithTag("Player").transform;
        BossPos = animator.GetComponent<BossStatus>().transform;
        agent = animator.GetComponent<NavMeshAgent>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
       
        float AttackRangeFromCLose = animator.GetComponent<BossStatus>().AttackRangeFromCLose;
        float AttackRangeFromFar = animator.GetComponent<BossStatus>().AttackRangeFromFar;

        if (Vector3.Distance(PlayerPos.position, BossPos.position) < AttackRangeFromCLose &&
            Vector3.Distance(PlayerPos.position, BossPos.position) < AttackRangeFromFar && !animator.GetBool("Phase 2"))
        {
            
            if (Phase1count >= FinalskillPhase1count)
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Skill1");
                animator.ResetTrigger("Skill2");
                animator.SetTrigger("Skill2");
                Phase1count = 0;
                return;
            }
            int random = Random.Range(0, 2);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Skill1");
            animator.ResetTrigger("Skill2");
            switch (random)
            {
                case 0:
                    animator.SetTrigger("Attack");
                    Phase1count++;
                    break;
                case 1:
                    animator.SetTrigger("Skill1");
                    Phase1count++;
                    break;               
            }
            return;
        }
        else if (Vector3.Distance(PlayerPos.position, BossPos.position) < AttackRangeFromCLose &&
            Vector3.Distance(PlayerPos.position, BossPos.position) < AttackRangeFromFar && animator.GetBool("Phase 2"))
        {

            if (Phase2count >= FinalskillPhase2count)
            {
                animator.ResetTrigger("Skill4");
                animator.ResetTrigger("Skill5");
                animator.ResetTrigger("Skill6");
                animator.ResetTrigger("Skill7");
                animator.SetTrigger("Skill7");
                Phase2count = 0;
                return;
            }
            int random = Random.Range(0, 5);
            animator.ResetTrigger("Skill4");
            animator.ResetTrigger("Skill5");
            animator.ResetTrigger("Skill6");
            animator.ResetTrigger("Skill7");
            switch (random)
            {
                case 0:
                    animator.SetTrigger("Skill4");
                    Phase2count++;
                    break;
                case 1:
                    animator.SetTrigger("Skill5");
                    Phase2count++;
                    break;
                case 2:
                    animator.SetTrigger("Skill6");
                    Phase2count++;
                    break;
                case 3:
                    animator.SetTrigger("Attack");
                    Phase2count++;
                    break;
            }
            return;
        }
        else if(Vector3.Distance(PlayerPos.position, BossPos.position) > AttackRangeFromCLose &&
            Vector3.Distance(PlayerPos.position, BossPos.position) < AttackRangeFromFar)
        {
            agent.SetDestination(PlayerPos.transform.position);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
           
    }
}
