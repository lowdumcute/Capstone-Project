using UnityEngine;

public class InputBlockManager : MonoBehaviour
{
    public static InputBlockManager Instance;

    // Biến này giữ trạng thái input có bị chặn không
    // true = bị chặn (không thể Attack), false = bình thường
    private bool isBlocked = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Chặn toàn bộ input Attack
    /// Gọi khi mở Dialogue, Map, Quest, Inventory... 
    /// </summary>
    public void BlockInput()
    {
        isBlocked = true;
    }

    /// <summary>
    /// Mở lại input Attack
    /// Gọi khi đóng Dialogue, Map, Quest, Inventory...
    /// </summary>
    public void UnblockInput()
    {
        isBlocked = false;
    }

    /// <summary>
    /// Kiểm tra xem có được phép Attack không
    /// PlayerAttack.cs sẽ gọi hàm này trước khi xử lý tấn công
    /// </summary>
    public bool CanAttack()
    {
        return !isBlocked;
    }
}
