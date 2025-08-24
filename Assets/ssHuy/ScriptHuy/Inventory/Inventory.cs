using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int space = 36; // số ô trong inventory
    public List<Item> items = new List<Item>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("Inventory full!");
            return false;
        }
        items.Add(item);
        // Update UI
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        // Update UI
    }
}
