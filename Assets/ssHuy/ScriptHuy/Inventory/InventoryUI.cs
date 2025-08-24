using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; // grid chứa các slot

    Inventory inventory;
    InventorySlot[] slots;

    void Start()
    {
        // test
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventory = Inventory.instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
