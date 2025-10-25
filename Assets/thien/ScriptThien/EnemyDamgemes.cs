using UnityEngine;

public class EnemyDamgemes : MonoBehaviour
{
    public EnemyHealth EnemyHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            
            if(EnemyHealth.currentHealth <= 500)
            {
                PlayerStats.Instance.TakeDamage(100);
            }
            else
            {
                PlayerStats.Instance.TakeDamage(EnemyHealth.damage);
            }
            Debug.Log("dame");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            if (EnemyHealth.currentHealth <= 500)
            {
                PlayerStats.Instance.TakeDamage(100);
            }
            else
            {
                PlayerStats.Instance.TakeDamage(EnemyHealth.damage);
            }
        }
    }
}
