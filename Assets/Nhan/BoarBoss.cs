using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class BoarBoss : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform BossModel;
    [SerializeField] private Transform Player;
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private Collider AttackCollider;
    [SerializeField] private float rotationSpeed = 6;
    //UI
    private void Start()
    {
        animator = BossModel.gameObject.GetComponent<Animator>();
    }
    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || !animator.GetBool("Dead"))
        {
            Vector3 RotationOffset = transform.position - Player.transform.position;
            RotationOffset.y = 0;
            float lookDirection = Vector3.SignedAngle(transform.forward, RotationOffset, Vector3.up);
            animator.SetFloat("Look Direction", lookDirection);
        }
        else if (!animator.GetBool("Attack") || animator.GetBool("CanRotate"))
        {
            var targetRotation = Quaternion.LookRotation(Player.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,rotationSpeed*Time.deltaTime);
        }
    }
}
