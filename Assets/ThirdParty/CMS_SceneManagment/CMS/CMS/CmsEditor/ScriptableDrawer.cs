#if UNITY_EDITOR
using CMS.Config;
using System.Reflection;
using UnityEngine;
namespace CMS.Editor
{
    public class ScriptableDrawer
    {
        public static object Draw(object obj, FieldInfo field)
        {
            if (!field.IsPublic)
                return obj;

            DrawAttribute drawType = field.GetCustomAttribute<DrawAttribute>();
            dynamic value = obj;
            
            if (drawType == null)
            {
                if (obj is IGUIDrawable)
                    return ScriptableGUIUtils.DrawField(field.Name, value as IGUIDrawable);
                else
                    return ScriptableGUIUtils.DrawField(field.Name, value);
            }
            else if (drawType.DrawAttributeTypes == DrawAttributeTypes.IdPopup)
            {
                return ScriptableGUIUtils.DrawIDPopup(drawType.Label, value);
            }
            else if (drawType.DrawAttributeTypes == DrawAttributeTypes.ObjectSellectionField)
            {
                return ScriptableGUIUtils.DrawObjectField(drawType.Label, value);
            }
            else if (drawType.DrawAttributeTypes == DrawAttributeTypes.ScriptableSelectionWindow)
            {
                return ScriptableGUIUtils.DrawScriptableSelectionWindow(drawType.Label, value);
            }
          
            return value;
        }
        /*
        public static void Draw(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                Draw(obj, fields[i]);
            }
            

            
        }
        */
    }
}
#endif
