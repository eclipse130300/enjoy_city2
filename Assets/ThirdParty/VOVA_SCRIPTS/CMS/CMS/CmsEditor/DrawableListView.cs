using CMS.Config;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Editor
{
    public class DrawableListView
    {
#if UNITY_EDITOR
        IScriptableListItem newSelectedItem;
        float itemInRaw = 1;
        private Vector2 scrollPos;

        public virtual IScriptableListItem DrawSellectableList<T>(List<T> list, T selectedItem, List<int> searchResult, int width = 130) where T : IScriptableListItem
        {
            GUILayout.BeginVertical();
            itemInRaw = Mathf.Clamp(width / 150, 1, int.MaxValue);
            if (list == null)
            {
                newSelectedItem = null;
            }
            newSelectedItem = selectedItem;

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(width));

            int selected = -1;
            List<GUIContent> content = new List<GUIContent>();

            for (int i = 0; i < list.Count; i++)
            {
                if (searchResult.Count == 0 || searchResult.Contains(i))
                {

                    content.Add(list[i].ContentForListMaximal());
                    if (selectedItem != null && list[i].ConfigId == selectedItem.ConfigId)
                        selected = content.Count - 1;
                }
            }

            selected = GUILayout.SelectionGrid(selected, content.ToArray(), Mathf.RoundToInt(itemInRaw), GUILayout.MaxWidth(150 * itemInRaw));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            if (selected < list.Count && selected >= 0)
            {
                if (searchResult.Count == 0 || list.Count == searchResult.Count)
                {
                    newSelectedItem = list[selected];
                }
                else
                {
                    newSelectedItem = list[searchResult[selected]];
                }
            }
            else
            {
               
            }

            return (T) newSelectedItem ;

        }
        private void Draw(IScriptableListItem Element, int width)
        {
            if (newSelectedItem == Element as IScriptableListItem)
            {
                GUI.backgroundColor = Color.cyan;
            }
            if (GUILayout.Button((Element as IScriptableListItem).ContentForListMaximal(), GUILayout.Width(width - 30), GUILayout.Height(40), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                newSelectedItem = Element;
            }
            GUI.backgroundColor = Color.white;
        }
#endif
    }
}
