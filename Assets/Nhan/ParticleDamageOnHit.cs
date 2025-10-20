using System.Collections.Generic;
using UnityEngine;

public class ParticleDamageOnHit : MonoBehaviour
{
    public float damagePerParticle = 10f; // sát thương mỗi sự kiện particle
    public bool multiplyByNumEvents = true;
    ParticleSystem ps;
    List<ParticleCollisionEvent> collisionEvents;
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }
    void OnParticleCollision(GameObject other)
    {
        int num = ps.GetCollisionEvents(other, collisionEvents);
        float damage = multiplyByNumEvents ? damagePerParticle * num : damagePerParticle;
        if (other.gameObject.CompareTag("Enemy"))
        {
            

            var dmg = other.GetComponent<EnemyHealth>();
            if (dmg != null)
            {
                dmg.TakeDamage(damagePerParticle);
                Debug.Log("Take DAme");
            }
            else
            {
                other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // Ví dụ: spawn hit VFX tại collisionEvents[0].intersection nếu cần
        // Vector3 hitPos = (num>0) ? collisionEvents[0].intersection : other.transform.position;
    }
    
}
