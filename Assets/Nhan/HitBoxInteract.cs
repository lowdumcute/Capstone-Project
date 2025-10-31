using UnityEngine;

public class HitBoxInteract : MonoBehaviour
{
    public float Dame;
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damage enemy with Trigger");
            var Enemytobject = other.gameObject.GetComponent<EnemyHealth>();
            Enemytobject.TakeDamage(Dame);
            Destroy(this.gameObject);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Damage enemy with coliision");
            var Enemytobject = collision.gameObject.GetComponent<EnemyHealth>();
            Enemytobject.TakeDamage(Dame);
            Destroy(this.gameObject);

        }
    }
}
