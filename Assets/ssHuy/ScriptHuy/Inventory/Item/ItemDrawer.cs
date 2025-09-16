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

        // Layout
        float iconSize = 40;
        float padding = 5;

        Rect iconRect = new Rect(position.x, position.y, iconSize, iconSize);
        Rect equippableRect = new Rect(position.x + iconSize + padding, position.y, 20, EditorGUIUtility.singleLineHeight);
        Rect nameRect = new Rect(position.x + iconSize + padding + 25, position.y, position.width - iconSize - 30, EditorGUIUtility.singleLineHeight);
        Rect amountRect = new Rect(position.x + iconSize + padding, position.y + EditorGUIUtility.singleLineHeight + 2,
                                   position.width - iconSize - padding, EditorGUIUtility.singleLineHeight);

        // Vẽ BaseStatsItem (ScriptableObject) để kéo thả
        EditorGUI.PropertyField(iconRect, baseStatsItemProp, GUIContent.none);

        // Vẽ checkbox isEquippable
        EditorGUI.PropertyField(equippableRect, isEquippableProp, GUIContent.none);

        // Vẽ tên item
        if (baseStatsItem != null)
        {
            GUI.Label(nameRect, itemName);

            if (icon != null)
            {
                GUI.DrawTexture(iconRect, icon.texture, ScaleMode.ScaleToFit);
            }
        }

        // Vẽ amount
        EditorGUI.PropertyField(amountRect, amountProp, new GUIContent("Amount"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // cao hơn vì có thêm amount
        return EditorGUIUtility.singleLineHeight * 2 + 8;
    }
}
