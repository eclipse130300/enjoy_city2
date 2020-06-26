#if UNITY_EDITOR

using System.Reflection;
using CMS.Config;
using CMS.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Utils;
using UnityEngine;
using UnityEditor.Animations;

namespace CMS.Editor
{
    public class ScriptableGUIUtils
    {

        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

        public static void DrawAllField(object obj) {
            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (var field in fields)
            {
                string name = field.Name;
                dynamic val = field.GetValue(obj);
                DrawAttribute attr = field.GetCustomAttribute<DrawAttribute>();
                if(attr == null)
                    field.SetValue(obj, DrawField(name, val));
                else
                    field.SetValue(obj, DrawAttrField(val, attr));
            }
        }

        public static object DrawAttrField(object value, DrawAttribute attr)
        {
            dynamic val = value;
            switch (attr.DrawAttributeTypes) {

                case DrawAttributeTypes.IdPopup:
                    return DrawIDPopup(attr.Label, val);
               
                case DrawAttributeTypes.ObjectSellectionField:
                    return DrawObjectField(attr.Label, val);
                    
                case DrawAttributeTypes.ScriptableSelectionWindow:
                    return DrawScriptableSelectionWindow(attr.Label, val);

                case DrawAttributeTypes.NotForDraw:
                    return val ;

                default:
                    return DrawField(attr.Label, val);
            }
        }

        public static int DrawField(string label, int value)
        {
            return EditorGUILayout.IntField(label, value);
        }
        public static float DrawField(string label, float value)
        {
            return EditorGUILayout.FloatField(label, value);
        }
        public static bool DrawField(string label, bool value)
        {
            return EditorGUILayout.Toggle(label, value);
        }
        public static string DrawField(string label, string value)
        {
            return EditorGUILayout.TextField(label, value);
        }
        public static Vector3 DrawField(string label, Vector3 value)
        {
            return EditorGUILayout.Vector3Field(label, value);
        }
        public static Vector2 DrawField(string label, Vector2 value)
        {
            return EditorGUILayout.Vector2Field(label, value);
        }
        public static Color DrawField(string label, Color value)
        {
            return EditorGUILayout.ColorField(label, value);
        }
        public static Enum DrawField(string label, Enum value)
        {
            return EditorGUILayout.EnumPopup(label, value);
        }
        public static Enum DrawFlagField(string label, Enum value)
        {
            return EditorGUILayout.EnumFlagsField(label, value);
        }
        public static IGUIDrawable DrawField(string label, IGUIDrawable value)
        {
            value.Draw();
            return value;
        }

        static Dictionary<object, bool> showContent = new Dictionary<object, bool>();
        public static List<BaseScriptableDrowableItem> DrawField(string label, List<BaseScriptableDrowableItem> value)
        {
            DrawList(label, value);
            return value;
        }

       // public static UnityEngine.Object DrawField(string label, UnityEngine.Object value)
        //{
       //     return EditorGUILayout.ObjectField(label, value, value.GetType(),false);
       // }
        public static T DrawObjectField<T>(string label, T obj) where T : UnityEngine.Object
        {
           
            return EditorGUILayout.ObjectField(label, obj, typeof(T), false) as T;
        }

        public static string DrawObjectField<T>(string label, string path) where T: UnityEngine.Object
        {
            return AssetDatabaseUtil.GetAssetPath(EditorGUILayout.ObjectField(label, Resources.Load(path), typeof(T), false));
        }

        public static bool DrawIDPopup<T>(string label, List<string> idList, bool showContent, Predicate<T> match = null) where T : BaseScriptableDrowableItem
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            showContent = GUILayout.Toggle(showContent, "showContent");
            GUILayout.EndHorizontal();
            if (!showContent)
                return showContent;
            for (int i = 0; i < idList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                idList[i] = DrawIDPopup<T>(string.Format("{0} {1}", label, i), idList[i], match);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    idList.RemoveAt(i);
                    i--;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Height(15)))
            {
                idList.Add("");
            }

            GUILayout.EndHorizontal();
            return showContent;

        }
        public static T DrawIDPopup<T>(string label, T item, Predicate<T> match = null) where T : BaseScriptableDrowableItem
        {
            return ScriptableList<T>.instance.GetItemByID(DrawIDPopup<T>(label, item.ConfigId, match));
        }
        public static string DrawIDPopup<T>(string label, string selected, Predicate<T> match = null) where T : BaseScriptableDrowableItem
        {
            List<string> popupContent = ScriptableList<T>.instance.GetIdList(match);
            popupContent.Sort();
            if (selected == "" || popupContent.IndexOf(selected) < 0)
            {
                selected = popupContent[0];
            }
            EditorGUILayout.LabelField(label);
            return popupContent[EditorGUILayout.Popup(Mathf.Clamp(popupContent.IndexOf(selected), 0, popupContent.Count), popupContent.ToArray())];

        }
        private static Vector2 scrollPosition;

        public static void DrawList<T>(string label, List<T> list) where T : IGUIDrawable, new()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);

            if (!showContent.ContainsKey(list))
                showContent.Add(list, false);

            showContent[list] = GUILayout.Toggle(showContent[list], String.Format("{0} ({1}) ", "showContent ", list.Count));
            GUILayout.EndHorizontal();

            if (!showContent[list])
                return;


            EditorGUILayout.Separator();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < list.Count; i++)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                list[i].Draw();

                GUILayout.Space(10);
                if (GUILayout.Button("-"))
                {
                    list.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Add", GUILayout.Height(15)))
            {
                list.Add(new T());
                if (list[list.Count - 1] is IScriptableListItem)
                {
                    (list[list.Count - 1] as IScriptableListItem).ConfigId = label + (list.Count - 1).ToString();
                }
                
            }
            GUILayout.Space(10);
            return;
        }

        public static void DrawList<T>(string label, List<T> list, T defaultValue) where T : Enum
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label);

            if (!showContent.ContainsKey(list))
                showContent.Add(list, false);

            showContent[list] = GUILayout.Toggle(showContent[list], String.Format("{0} ({1}) ", "showContent ", list.Count));
            GUILayout.EndHorizontal();

            if (!showContent[list])
                return;


            EditorGUILayout.Separator();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < list.Count; i++)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
/*                DrawField(label, list[i]);*/

                list[i] = (T) DrawField(label, list[i]);

                GUILayout.Space(10);
                if (GUILayout.Button("-"))
                {
                    list.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Add", GUILayout.Height(15)))
            {
                list.Add(defaultValue);
                if (list[list.Count - 1] is IScriptableListItem)
                {
                    (list[list.Count - 1] as IScriptableListItem).ConfigId = label + (list.Count - 1).ToString();
                }

            }
            GUILayout.Space(10);
            return;
        }

        static ScriptableListWindow window;
        static Vector2 scrollPos = new Vector2();

        public static void DrawScriptableSelectionWindow<T>(string label, List<T> idList, Predicate<T> match = null) where T : BaseScriptableDrowableItem
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);

            if (!showContent.ContainsKey(idList))
                showContent.Add(idList, false);

            showContent[idList] = GUILayout.Toggle(showContent[idList], String.Format("{0} ({1}) ", "showContent ", idList.Count));
            GUILayout.EndHorizontal();
            if (!showContent[idList])
                return;


            for (int i = 0; i < idList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                T sellectedItem = null;
                DrawScriptableSelectionWindow<T>(String.Format("{0} {1}", label, i.ToString()), idList[i], out sellectedItem);
                if (sellectedItem != null)
                {
                    idList[i] = sellectedItem;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    idList.RemoveAt(i);
                    i--;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Height(15)))
            {
                idList.Add(null);
            }

            GUILayout.EndHorizontal();
        }
        public static bool DrawScriptableSelectionWindow<T>(string label, List<T> idList, bool showContent, Predicate<T> match = null) where T : BaseScriptableDrowableItem
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            showContent = GUILayout.Toggle(showContent, String.Format("{0} ({1}) ", "showContent ", idList.Count));
            GUILayout.EndHorizontal();
            if (!showContent)
                return showContent;
            for (int i = 0; i < idList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                T sellectedItem = null;
                DrawScriptableSelectionWindow<T>(String.Format("{0} {1}", label, i.ToString()), idList[i], out sellectedItem);
                if (sellectedItem != null)
                {
                    idList[i] = sellectedItem;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    idList.RemoveAt(i);
                    i--;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Height(15)))
            {
                idList.Add(null);
            }

            GUILayout.EndHorizontal();
            return showContent;
        }
        public static string DrawScriptableSelectionWindow<T>(string label, string selected, out T sellectedItem) where T : BaseScriptableDrowableItem
        {
            T item = ScriptableList<T>.instance.GetItemByID(selected);

            return DrawScriptableSelectionWindow<T>(label, item, out sellectedItem);
        }
        public static T DrawScriptableSelectionWindow<T>(string label, T selected) where T : BaseScriptableDrowableItem
        {
            GUILayout.BeginHorizontal();
            GUIContent sellectedItemId = new GUIContent("NONE");
            if (selected != null)
            {
                sellectedItemId = selected.ContentForListMaximal();
            }
            GUILayout.Label(label, GUILayout.Width(200));

            if (GUILayout.Button(sellectedItemId, GUILayout.Width(200), GUILayout.MaxWidth(200), GUILayout.MaxHeight(25)))
            {
                if (window != null)
                {
                    window.Close();
                }
                window = ScriptableListWindow.ShowWindow<T>(label, selected);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

           T sellectedItem = ScriptableListWindow.GetSelectedItem(label) as T;

            if (sellectedItem != null)
            {

                return sellectedItem;
            }

            else
            {
                return selected;
            }
        }
        public static string DrawScriptableSelectionWindow<T>(string label, T selected, out T sellectedItem) where T : BaseScriptableDrowableItem
        {
            GUILayout.BeginHorizontal();
            GUIContent sellectedItemId = new GUIContent("NONE");
            if (selected != null)
            {
                sellectedItemId = selected.ContentForListMaximal();
            }
            GUILayout.Label(label, GUILayout.Width(200));

            if (GUILayout.Button(sellectedItemId, GUILayout.Width(200), GUILayout.MaxWidth(200), GUILayout.MaxHeight(25)))
            {
                if (window != null)
                {
                    window.Close();
                }
                window = ScriptableListWindow.ShowWindow<T>(label, selected);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            sellectedItem = ScriptableListWindow.GetSelectedItem(label) as T;

            if (sellectedItem != null)
            {

                return sellectedItem.ConfigId;
            }

            else
            {
                return "NONE";
            }
        }
        public static void Separator()
        {

            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        public static GameObject DrawObjectFieldWithVertexCount(string lable, GameObject target)
        {
            EditorGUILayout.BeginHorizontal();
            GameObject obj = DrawObjectField(lable, target);
            if (obj != null)
                DrawTotalMeshVertexCount(obj);
            EditorGUILayout.EndHorizontal();

            return obj;
        }
        public static void DrawTotalMeshVertexCount(GameObject target)
        {
            int vertex_count = GetTotalVertexCount(target);
            GUILayout.Label($"vertex count: {vertex_count}");
        }
        static int GetTotalVertexCount(GameObject target)
        {
            int vertex_count = 0;
            MeshFilter meshFilter = target.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null) vertex_count += meshFilter.sharedMesh.vertexCount;

            SkinnedMeshRenderer skinnedMesh = target.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh != null && skinnedMesh.sharedMesh !=null) vertex_count += skinnedMesh.sharedMesh.vertexCount;

            for (int i = 0; i < target.transform.childCount; i++)
                vertex_count += GetTotalVertexCount(target.transform.GetChild(i).gameObject);

            return vertex_count;
        }

        public static GameObject DrawMeshAnimObjectField(string lable, GameObject target)
        {
            EditorGUILayout.BeginHorizontal();
            GameObject obj = DrawObjectField(lable, target);
            if (obj != null)
                DrawTotalMeshVertexCount(obj);
            EditorGUILayout.EndHorizontal();

            if (obj != null)
            {
                string animNames = DrawObjectAnimationsNames(obj);
                if (animNames != string.Empty)
                    EditorGUILayout.HelpBox($"OBJECT ANIMATION CLIPS NAMES:\n\n{animNames}", MessageType.Info);
            }

            return obj;
        }
        static string DrawObjectAnimationsNames(GameObject target)
        {
            string animNames = "";
            Animator animator = target.GetComponentInChildren<Animator>();
            if (animator)
            {
                AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
                if (controller != null)
                {
                    foreach (var item in controller.animationClips)
                        animNames += $"{item}\n";
                }
            }
            return animNames;
        }
    }
}
#endif