using System.Collections;
using UnityEngine;

public class HS_FrontAttack : MonoBehaviour
{
    public Transform pivot;
    public Vector3 startRotation;
    public float speed = 15f;
    public float drag = 1f;
    public GameObject craterPrefab;
    public ParticleSystem ps;
    public bool playPS = false;
    public float spawnRate = 1f;
    public float spawnDuration = 1f;
    public float positionOffset = 2f; // Khoảng cách spawn trước mặt player
    public bool changeScale = false;

    [Header("Ground Detection")]
    public LayerMask groundLayer = 1; // Layer mặt đất
    public float groundCheckDistance = 10f;

    private float randomTimer = 0f;
    private float startSpeed = 0f;
    private Vector3 stepPosition;
    private Vector3 attackDirection;

    [Space]
    [Header("Effect with Mesh animation")]
    public bool effectWithAnimation = false;
    public Animator[] anim;
    public float delay = 0f;
    public bool playMeshEffect;

    private void Update()
    {
        if (playMeshEffect == true)
        {
            StartCoroutine(MeshEffect());
            playMeshEffect = false;
        }
    }

    public void PrepeareAttack(Vector3 targetPoint)
    {
        if (effectWithAnimation)
        {
            StartCoroutine(MeshEffect());
        }
        else
        {
            if (playPS) ps.Play();
            startSpeed = speed;
            transform.parent = null;
            transform.position = pivot.position;

            var lookPos = targetPoint - transform.position;
            lookPos.y = 0;
            attackDirection = lookPos.normalized; // Lưu hướng tấn công

            if (!playPS)
            {
                transform.rotation = Quaternion.LookRotation(lookPos);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(lookPos) * Quaternion.Euler(startRotation);
            }

            stepPosition = pivot.position;
            randomTimer = 0;
            StartCoroutine(StartMove());
        }
    }

    public IEnumerator MeshEffect()
    {
        if (playPS) ps.Play();
        yield return new WaitForSeconds(delay);
        foreach (var animS in anim)
        {
            animS.SetTrigger("Attack");
        }
        yield break;
    }

    public IEnumerator StartMove()
    {
        randomTimer = 0f;

        while (randomTimer < spawnDuration)
        {
            randomTimer += Time.deltaTime;
            startSpeed *= drag;

            // Di chuyển về phía trước
            transform.position += transform.forward * (startSpeed * Time.deltaTime);

            // Kiểm tra khoảng cách để spawn crater
            var heading = transform.position - stepPosition;
            var distance = heading.magnitude;

            if (distance > spawnRate)
            {
                SpawnCrater();
                stepPosition = transform.position;
            }

            yield return null;
        }

        // Kết thúc, reset về pivot
        transform.parent = pivot;
        transform.position = pivot.position;
        transform.rotation = Quaternion.Euler(startRotation);
    }

    private void SpawnCrater()
    {
        if (craterPrefab == null) return;

        // Tính vị trí spawn TRƯỚC MẶT player theo hướng tấn công
        Vector3 spawnPosition = CalculateSpawnPosition();

        // Tạo crater
        var craterInstance = Instantiate(craterPrefab, spawnPosition, Quaternion.identity);

        // Đảm bảo crater luôn hướng lên trên
        craterInstance.transform.up = Vector3.up;

        // Thay đổi scale nếu được yêu cầu
        if (changeScale)
        {
            float scaleMultiplier = 1f + randomTimer;
            craterInstance.transform.localScale = Vector3.one * scaleMultiplier;
        }

        // Tự hủy sau khi hiệu ứng kết thúc
        StartCoroutine(DestroyAfterEffect(craterInstance));
    }

    private Vector3 CalculateSpawnPosition()
    {
        // Vị trí cơ sở: trước mặt player
        Vector3 basePosition = transform.position + attackDirection * positionOffset;

        // Thêm ngẫu nhiên nhỏ
        Vector3 randomOffset = new Vector3(
            Random.Range(-positionOffset * 0.3f, positionOffset * 0.3f),
            0,
            Random.Range(-positionOffset * 0.2f, positionOffset * 0.2f)
        );

        Vector3 finalPosition = basePosition + randomOffset;

        // Đảm bảo crater spawn trên mặt đất
        finalPosition = GetGroundPosition(finalPosition);

        return finalPosition;
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        // Sử dụng Raycast để tìm vị trí mặt đất
        RaycastHit hit;
        Vector3 rayStart = position + Vector3.up * groundCheckDistance;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundCheckDistance * 2, groundLayer))
        {
            return hit.point + Vector3.up * 0.1f; // Nâng lên một chút để không bị chìm
        }

        // Fallback: nếu không tìm thấy mặt đất, đặt ở độ cao 0
        position.y = 0.1f;
        return position;
    }

    private IEnumerator DestroyAfterEffect(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration);
            Destroy(effect);
        }
        else
        {
            // Nếu không có ParticleSystem, tìm trong children
            ParticleSystem childPs = effect.GetComponentInChildren<ParticleSystem>();
            if (childPs != null)
            {
                yield return new WaitForSeconds(childPs.main.duration);
                Destroy(effect);
            }
            else
            {
                // Fallback: hủy sau 3 giây
                yield return new WaitForSeconds(3f);
                Destroy(effect);
            }
        }
    }

    // Helper method để vẽ Gizmos trong Scene view (tùy chọn)
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, attackDirection * positionOffset);

            Gizmos.color = Color.green;
            Vector3 spawnPos = CalculateSpawnPosition();
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
        }
    }
}