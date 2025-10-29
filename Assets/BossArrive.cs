using UnityEngine;

public class BossArrive : MonoBehaviour
{
    public Animator animator;
    public GameObject Boss;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            animator.Play("n2017_born");
            Boss.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }    
    }
}
