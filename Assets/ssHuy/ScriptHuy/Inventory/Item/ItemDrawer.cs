using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Item))]
public class ItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Lấy property con
        var baseStatsItemProp = property.FindPropertyRelative("BaseStatsItem");
        var isEquippableProp = property.FindPropertyRelative("isEquippable");
        var amountProp = property.FindPropertyRelative("amount");

        BaseItem baseStatsItem = baseStatsItemProp.objectReferenceValue as BaseItem;

        string itemName = baseStatsItem != null ? baseStatsItem.itemName : "None";
        Sprite icon = baseStatsItem != null ? baseStatsItem.icon : null;
        string itemType = baseStatsItem != null ? baseStatsItem.itemType.ToString() : "Unknown";

        // Layout
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float padding = 4f;
        float iconSize = 40;

        // --- Hàng 1: Icon + BaseStatsItem (object field) + tên item ---
        Rect iconRect = new Rect(position.x, position.y, iconSize, iconSize);
        Rect objectFieldRect = new Rect(position.x + iconSize + padding, position.y, position.width - iconSize - padding, lineHeight);
        Rect nameRect = new Rect(position.x + iconSize + padding, position.y + lineHeight + 2, position.width - iconSize - padding, lineHeight);

        EditorGUI.PropertyField(objectFieldRect, baseStatsItemProp, GUIContent.none);

        if (baseStatsItem != null)
        {
            GUI.Label(nameRect, itemName, EditorStyles.boldLabel);

            if (icon != null)
            {
                GUI.DrawTexture(iconRect, icon.texture, ScaleMode.ScaleToFit);
            }
        }
        else
        {
            GUI.Label(nameRect, "No Item", EditorStyles.miniLabel);
        }

        // --- Hàng 2: isEquippable ---
        Rect equippableRect = new Rect(position.x, position.y + iconSize + padding, position.width, lineHeight);
        EditorGUI.PropertyField(equippableRect, isEquippableProp, new GUIContent("Is Equippable"));

        // --- Hàng 3: item type (cho phép chọn enum) ---
        Rect typeRect = new Rect(position.x, equippableRect.y + lineHeight + 2, position.width, lineHeight);
        var itemTypeProp = property.FindPropertyRelative("itemType");
        EditorGUI.PropertyField(typeRect, itemTypeProp, new GUIContent("Item Type"));


        // --- Hàng 4: amount ---
        Rect amountRect = new Rect(position.x, typeRect.y + lineHeight + 2, position.width, lineHeight);
        EditorGUI.PropertyField(amountRect, amountProp, new GUIContent("Amount"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 4 dòng: Icon block (~40), isEquippable, ItemType, Amount
        return EditorGUIUtility.singleLineHeight * 4 + 55;
    }
}
