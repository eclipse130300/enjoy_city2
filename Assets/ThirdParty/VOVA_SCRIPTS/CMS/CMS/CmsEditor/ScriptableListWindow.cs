using CMS.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace CMS.Editor
{
    public class ScriptableListWindow : EditorWindow
    {
        //public static List<IGUIDrawable> data = new List<IGUIDrawable>();

        DrawableListViewWithSearchBar listWiew = new DrawableListViewWithSearchBar();
        static Dictionary<string, IScriptableListItem> selectedItems = new Dictionary<string, IScriptableListItem>();
        static IScriptableListItem selectedItem;
        List<IGUIDrawable> loadedData;

        delegate IScriptableListItem DrawList(IScriptableListItem sellected);
        DrawList drawList;
        static string windowId = "";

        public static IScriptableListItem GetSelectedItem(string lableId)
        {
            IScriptableListItem outItem;
            if (selectedItems.TryGetValue(lableId, out outItem))
            {
                if (outItem != null)
                {
                    selectedItems.Remove(lableId);
                    return outItem;
                }
            }
            return null;
        }
        public static ScriptableListWindow ShowWindow<T>(string id, IScriptableListItem selected) where T : BaseScriptableDrowableItem
        {
            windowId = id;

            EditorWindow window = EditorWindow.GetWindow(typeof(ScriptableListWindow));
            if (selectedItems.ContainsKey(windowId))
                selectedItems.Remove(windowId);

            selectedItems.Add(windowId, selected);
            selectedItem = selected;
            (window as ScriptableListWindow).drawList = (IScriptableListItem sellected) =>
            {
                return (window as ScriptableListWindow).listWiew.DrawSellectableList<T>(ScriptableList<T>.instance.list, selectedItem as T,
                  Mathf.RoundToInt((window as ScriptableListWindow).position.width));
            };
            return (window as ScriptableListWindow);
        }
        public void OnGUI()
        {
            IScriptableListItem newSelectedItem = drawList(selectedItem as IScriptableListItem);

            if (newSelectedItem != selectedItem && newSelectedItem != null)
            {
                selectedItems[windowId] = newSelectedItem;
                Close();
            }

        }
    }
}
#endif
