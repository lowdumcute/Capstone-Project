using UnityEngine;

public class damageR : MonoBehaviour
{
    public float Dame;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damage enemy with Trigger");
            var Enemytobject = other.gameObject.GetComponent<EnemyHealth>();
            Enemytobject.TakeDamage(Dame);

        }
    }
}
