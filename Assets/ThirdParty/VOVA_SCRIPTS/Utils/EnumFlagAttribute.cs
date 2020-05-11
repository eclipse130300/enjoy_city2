using UnityEngine;
#if UNITY_EDITOR
// NOTE(Rostislav Pogosian 21.11.2018): Для EnumFlagDrawer.
using UnityEditor; 
#endif

namespace Utils.Attributes
{
    /// <summary>
    /// Аттрибут для работы с флагами в инспекторе. 
    /// </summary>
    public class EnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagAttribute() { }

        public EnumFlagAttribute(string name)
        {
            enumName = name;
        }
    }

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
         property.intValue = EditorGUI.MaskField(rect, label, property.intValue, property.enumNames);
    }
}

#endif
}