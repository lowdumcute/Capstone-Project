using Unity.VisualScripting;
using UnityEngine;

public class AnimtionActionController : StateMachineBehaviour
{
    EnemyController EnemyCol;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         EnemyCol = animator.gameObject.GetComponent<EnemyController>();
         EnemyCol.InAction = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCol.InAction = false;
    }
}
