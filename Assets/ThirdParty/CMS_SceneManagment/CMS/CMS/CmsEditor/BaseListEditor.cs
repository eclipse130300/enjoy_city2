using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMS.Config;

#if UNITY_EDITOR
using UnityEditor;
namespace CMS.Editor
{
    public class BaseListEditor<U> : EditorWindow where U : BaseScriptableDrowableItem, IGUIDrawable,IScriptableListItem
    {
        ScriptableList<U> drawableList;
        List<int> searchResult = new List<int>();
        public bool temporaryRemove = false;
        private U _viewElement = null;
        private DrawableListViewWithSearchBar drawListView = new DrawableListViewWithSearchBar();
        private DrowableListSearchBarView drowableListSearchBarView = new DrowableListSearchBarView();

        public string ViewElementKey
        {
            get
            {
                if (_viewElement != null)
                {
                    return _viewElement.ConfigId;
                }
                return "";
            }
            set
            {
                var item = drawableList.GetItemByID(value);
                if (item != null)
                {
                    viewElement = item;
                }
            }
        }

        public U viewElement
        {
            get
            {
                if (_viewElement == null)
                {
                    viewIndex = 0;
                }
                return _viewElement;
            }
            set
            {
                _viewElement = value;
                viewIndex = drawableList.list.IndexOf(value as U);
            }
        }
        public int viewIndex
        {
            get
            {
                return drawableList.list.IndexOf(viewElement as U);
            }
            set
            {
                if (drawableList.list.Count > 0)
                {
                    _viewElement = drawableList[Mathf.Clamp(value, 0, drawableList.list.Count - 1)];
                }
            }
        }
        void OnEnable()
        {
            if (drawableList == null)
                drawableList = new ScriptableList<U>();

        }
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            DrawList();
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (drawableList != null)
            {
                if (GUILayout.Button("Add Item", GUILayout.ExpandWidth(false)))
                {
                    AddItem();
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
                {
                    viewIndex--;
                    GUI.FocusControl("clearFocus");
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
                {
                    viewIndex++;
                    GUI.FocusControl("clearFocus");
                }
                if (temporaryRemove)
                {
                    GUI.backgroundColor = Color.red;
                }
                if (drawableList.list.Count > 0 && GUILayout.Button("Delete Item", GUILayout.ExpandWidth(false)))
                {
                    DeleteItem(viewIndex);
                }
                GUI.backgroundColor = Color.white;
                if (drawableList.list.Count == 0)
                {
                    return;
                }
                temporaryRemove = GUILayout.Toggle(temporaryRemove, "FromDisk", GUILayout.ExpandWidth(false));
                GUILayout.Space(5);
                GUILayout.FlexibleSpace();
                //DrawSearchBar();

            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (drawableList.list.Count > 0)
            {
                drawableList[viewIndex].Draw();
                if (GUI.changed)
                    drawableList[viewIndex].Save();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();


        }


        private void DrawSearchBar()
        {
            searchResult = drowableListSearchBarView.DrawSearchBar(drawableList.list);
        }
        private void DrawList()
        {
            U newItem = drawListView.DrawSellectableList(drawableList.list, viewElement, Mathf.RoundToInt(position.width / 5)) as U ;
            if (newItem != viewElement)
            {
                viewElement = newItem;
                GUI.FocusControl(null);
            }


        }
        public void OnInspectorUpdate()
        {
            if (drawableList != null && drawableList.list != null)
            {
                for (int i = 0; i < drawableList.list.Count; i++)
                {
                    if (drawableList[i] == Selection.activeObject && viewElement != drawableList[i])
                    {
                        viewIndex = i;

                        Selection.activeObject = null;
                        this.Focus();
                        Repaint();
                    }
                }
            }
        }


        void AddItem()
        {

            viewElement = drawableList.AddItem();
        }

        void DeleteItem(int index)
        {
            drawableList.DeleteItem(index, temporaryRemove);
            viewIndex--;
        }
        private void OnDestroy()
        {
            drawableList.SaveAllNewItem();
            viewElement = null;
            drawListView = null;
        }
    }
}
#endif
