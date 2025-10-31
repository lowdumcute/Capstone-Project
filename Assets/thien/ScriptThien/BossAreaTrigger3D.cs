using UnityEngine;

public class BossAreaTrigger3D : MonoBehaviour
{
    [Header("Boss Area Settings")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;

    [Header("Audio Manager")]
    public AudioManager audioManager;

    void Start()
    {
        // Tự động tìm AudioManager nếu chưa gán
        if (audioManager == null)
        {
            audioManager = FindAnyObjectByType<AudioManager>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu trigger chỉ hoạt động 1 lần và đã kích hoạt rồi
        if (triggerOnce && hasTriggered) return;

        // Kích hoạt khi PLAYER vào vùng boss
        if (other.CompareTag("Player"))
        {
            TriggerBossMusic();
        }
    }

    private void TriggerBossMusic()
    {
        if (audioManager != null)
        {
            audioManager.StartBossMusic();
            hasTriggered = true;
            Debug.Log("Player entered boss area! Switching to boss music.");
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
    }

  
    // Hiển thị Gizmo trong Scene để dễ nhìn thấy trigger
    void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null && boxCollider.enabled)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Màu đỏ trong suốt
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            // Viền màu đỏ đậm
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }

    void OnDrawGizmosSelected()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null && boxCollider.enabled)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Màu cam khi selected
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
    }
}