using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    private HS_CameraShaker cameraShaker;

    void Start()
    {
        cameraShaker = FindObjectOfType<HS_CameraShaker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DamageCollider"))
        {
            Debug.Log("Being attacked");
            TakeDamage(12);
        }
    }
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            // chết
        }

        // gọi rung camera khi bị đánh
        if (cameraShaker != null)
        {
            cameraShaker.TriggerShake(0.5f, 20f, 0.2f);
            // amp = độ rung
            // freq = tần số
            // dur = thời gian
            Debug.Log("playee");
        }
    }
}
