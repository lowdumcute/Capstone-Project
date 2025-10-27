using UnityEngine;

public class HitBoxInteract : MonoBehaviour
{
    public AttackCameraEffects attackCameraEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit And Start Shake");
         
        }
    }
}
