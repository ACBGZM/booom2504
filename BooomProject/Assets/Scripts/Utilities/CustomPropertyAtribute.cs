#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class ReadOnlyAttribute : PropertyAttribute
{
    // This is an empty class, simply used as a marker for the attribute
}

public class ReadOnlyIfFalseAttribute : PropertyAttribute
{
    public string boolPropertyName;

    public ReadOnlyIfFalseAttribute(string boolPropertyName)
    {
        this.boolPropertyName = boolPropertyName;
    }
}

public class LabelAttribute : PropertyAttribute
{
    private readonly string name = "";
    public string Name => name;

    public LabelAttribute(string name)
    {
        this.name = name;
    }
}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;  // Disable editing the field
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;   // Enable editing for other fields
    }
}

[CustomPropertyDrawer(typeof(ReadOnlyIfFalseAttribute))]
public class ReadOnlyIfFalseDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReadOnlyIfFalseAttribute readOnlyIfFalseAttribute = (ReadOnlyIfFalseAttribute)attribute;
        SerializedProperty boolProperty = property.serializedObject.FindProperty(readOnlyIfFalseAttribute.boolPropertyName);

        if (boolProperty != null && !boolProperty.boolValue)
        {
            GUI.enabled = false;
        }

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = true;
    }
}

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LabelAttribute labelAttribute = (LabelAttribute)attribute;
        if (labelAttribute.Name != "")
        {
            label.text = labelAttribute.Name;
        }
        EditorGUI.PropertyField(position, property, label);
    }
}

#endif
