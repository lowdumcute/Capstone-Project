using System.Collections;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance;
    public GameObject target;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator TeleportTo()
    {
        if (target == null)
        {
            Debug.LogWarning(" Không có vị trí đích để dịch chuyển!");
            yield break;
        }

        var player = GameManager.Instance?.Player;
        if (player == null)
        {
            Debug.LogWarning(" Không tìm thấy Player để dịch chuyển!");
            yield break;
        }

        // Bắt đầu che màn hình
        SceneChangeManager.Instance.OpenBlackScreenAnimator();

        // Đợi 1 giây cho hiệu ứng fade in hoàn tất
        yield return new WaitForSeconds(1f);

        // Dịch chuyển player
        player.transform.position = target.transform.position;

        Debug.Log($" Dịch chuyển Player đến {target.name} tại {target.transform.position}");

        // Mở lại màn hình
        SceneChangeManager.Instance.CloseBlackScreenAnimator();
    }
}

